## Background ##

This is a test of your ability to convert some requirements into a small application. We will examine your solution to get an idea of your current strengths and weaknesses.

### Notes on the test ###

● Anyone suspected of using AI will not be considered for the role.

● We expect a command line application

● The solution should be submitted via a link to a Github repository (discuss other arrangements in advance if this is not possible for some reason)

● We have preference for code written in F#. C# is acceptable. Other languages may be allowed for junior developers upon request.

● The code must work in Visual Studio 2026 without custom setup.

● The test is expected to take 1-2 hours of time.

● Do not use any commercially licensed libraries. The problems we set do not require them.

● Inclusion of unit tests is up to your judgement

## Scenario ##

We write lots of SQL queries. It’s very useful to have a library that allows you to build SQL programmatically (but lighter weight than an ORM). We want you to build and demonstrate a library that supports the following features:

● Builds a query using code objects

● Turn the code objects into valid (MS/ANSI) SQL

● SELECTs from any table

● Supports column and table aliases

● With a dynamic list of fields

● Supports multiple INNER and OUTER JOINS

● Supports a WHERE clause

○ This clause allows AND and ORs to combine multiple criteria into one


NOTE: it doesn’t need to execute queries, just produce the SQL.


We will be judging this on the following criteria, amongst others:

● It produces valid (MS) T-SQL

● The library works with auto-completion and Intellisense

● Preference of strongly-typed code over magic strings

● Ease of use and understanding

● Maintainability

● Ability to construct all sorts of SQL statements matching the requirements - not hard-coded in the implementation.

● Multiple working examples

● Demonstration of solid professional development skills


This is an example abstract query to build using your library. The outputted SQL will be different:

From: Events

Inner Join: Events → EventAttendee, EventAttendee → Attendee

Where: Attendee.Name = “bob” or Events.Important = 1