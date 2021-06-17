using LogicMonitor.Api;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Datamart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart.Extensions
{
	public static class DbSetExtension
	{
		public static void AddOrUpdateIdentifiedItem<TApi, TStore>(
				this DbSet<TStore> dbSet,
				TApi data,
				DateTime lastObservedUtc,
				ILogger logger
			)
			where TApi : IdentifiedItem
			where TStore : IdentifiedStoreItem
		{
			if (dbSet == null)
			{
				throw new ArgumentNullException(nameof(dbSet));
			}
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var storeItem = dbSet.FirstOrDefault(si => si.Id == data.Id);
			if (storeItem != null)
			{
				// Update an existing entry
				if (logger?.IsEnabled(LogLevel.Trace) == true)
				{
					logger.LogTrace($"Updating existing {typeof(TStore).Name} with id {storeItem.Id} ({storeItem.DatamartId})");
				}
				// Map from data onto the existing storeItem which EF internal tracker will work out whether anything changed
				storeItem = DatamartClient.MapperInstance.Map(data, storeItem);
				storeItem.DatamartLastObservedUtc = lastObservedUtc;
				return;
			}
			if (logger?.IsEnabled(LogLevel.Trace) == true)
			{
				logger.LogTrace($"Adding new {typeof(TStore).Name} with id {data.Id}");
			}

			// Add a new entry
			var newEntry = DatamartClient.MapperInstance.Map<TApi, TStore>(data);
			newEntry.DatamartLastObservedUtc = lastObservedUtc;
			dbSet.Add(newEntry);
		}

		public static async Task AddOrUpdateAlertRangeSavingChanges(this DbSet<AlertStoreItem> dbSet, List<Alert> items)
		{
			foreach (var item in items ?? throw new ArgumentNullException(nameof(items)))
			{
				dbSet.AddOrUpdateAlert(item);
				await dbSet.GetContext()
				  .SaveChangesAsync()
				  .ConfigureAwait(false);
			}
		}

		public static void AddOrUpdateAlert(this DbSet<AlertStoreItem> dbSet, Alert data)
		{
			var context = dbSet.GetContext();
			var storeItem = dbSet.AsQueryable().Where(si => si.Id == data.Id).FirstOrDefault();
			var mappedStoreItem = DatamartClient.MapperInstance.Map<Alert, AlertStoreItem>(data);

			if (storeItem != null)
			{
				// Keep the existing Guid
				mappedStoreItem.DatamartId = storeItem.DatamartId;
				context.Entry(storeItem).CurrentValues.SetValues(mappedStoreItem);
				context.Entry(storeItem).State = EntityState.Modified;
				mappedStoreItem.DatamartLastModifiedUtc = DateTime.UtcNow;
				return;
			}
			else
			{
				mappedStoreItem.DatamartCreatedUtc = DateTime.UtcNow;
				mappedStoreItem.DatamartLastModifiedUtc = DateTime.UtcNow;
			}
			dbSet.Add(mappedStoreItem);
		}

		public static void AddOrUpdate<TApi, TStore>(this DbSet<TStore> dbSet, Expression<Func<TStore, object>> key, TApi data)
			where TApi : class
			where TStore : class
		{
			var context = dbSet.GetContext();
			var ids = context.Model.FindEntityType(typeof(TStore)).FindPrimaryKey().Properties.Select(x => x.Name);
			var t = typeof(TStore);
			var keyObject = key.Compile()(DatamartClient.MapperInstance.Map<TApi, TStore>(data));
			var keyFields = keyObject.GetType().GetProperties().Select(p => t.GetProperty(p.Name)).ToArray();
			if (keyFields == null)
			{
				throw new NotSupportedException($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
			}
			var keyVals = keyFields.Select(p => p.GetValue(data));
			var entities = dbSet.AsQueryable();
			var i = 0;
			foreach (var keyVal in keyVals)
			{
				entities = entities.Where(p => p.GetType().GetProperty(keyFields[i].Name).GetValue(p).Equals(keyVal));
				i++;
			}
			var dbVal = entities.FirstOrDefault();
			if (dbVal != null)
			{
				var keyAttrs =
					data.GetType().GetProperties().Where(p => ids.Contains(p.Name)).ToList();
				if (keyAttrs.Count > 0)
				{
					foreach (var keyAttr in keyAttrs)
					{
						keyAttr.SetValue(data,
							Array.Find(dbVal.GetType()
								.GetProperties(), p => p.Name == keyAttr.Name)
								.GetValue(dbVal));
					}
					context.Entry(dbVal).CurrentValues.SetValues(data);
					context.Entry(dbVal).State = EntityState.Modified;
					return;
				}
			}
			dbSet.Add(DatamartClient.MapperInstance.Map<TApi, TStore>(data));
		}
	}

	public static class HackyDbSetGetContextTrick
	{
		public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
			where TEntity : class => (DbContext)dbSet
				.GetType().GetTypeInfo()
				.GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(dbSet);
	}
}
