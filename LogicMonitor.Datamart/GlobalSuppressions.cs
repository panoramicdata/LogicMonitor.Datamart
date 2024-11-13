// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Performance",
	"CA1848:Use the LoggerMessage delegates",
	Justification = "Implementation effort not worth the gain",
	Scope = "namespaceanddescendants",
	Target = "~N:LogicMonitor.Datamart")
]
[assembly: SuppressMessage(
	"Design",
	"CA1062:Validate arguments of public methods",
	Justification = "Code generated",
	Scope = "namespaceanddescendants",
	Target = "~N:LogicMonitor.Datamart.Migrations")
]
[assembly: SuppressMessage(
	"Style",
	"IDE0022:Use expression body for method",
	Justification = "Code generated",
	Scope = "namespaceanddescendants",
	Target = "~N:LogicMonitor.Datamart.Migrations")
]
[assembly: SuppressMessage(
	"Naming",
	"CA1707:Identifiers should not contain underscores",
	Justification = "Code generated",
	Scope = "namespaceanddescendants",
	Target = "~N:LogicMonitor.Datamart.Migrations")
]
