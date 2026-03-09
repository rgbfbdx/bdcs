using Dapper;
using Microsoft.Data.SqlClient;

string connectionString =
    "Server=(localdb)\\mssqllocaldb;Database=BoardGamesClub;Trusted_Connection=True";

using var connection = new SqlConnection(connectionString);

var sessions = connection.Query(@"
SELECT g.Title, m.FullName, s.Date, s.DurationMinutes
FROM Sessions s
JOIN Games g ON s.GameId = g.Id
JOIN Members m ON s.MemberId = m.Id
");

foreach (var s in sessions)
{
    Console.WriteLine($"{s.Title} | {s.FullName} | {s.Date} | {s.DurationMinutes} min");
}

var topGames = connection.Query(@"
SELECT TOP 3 g.Title,
SUM(s.DurationMinutes) / 60.0 AS HoursPlayed
FROM Sessions s
JOIN Games g ON s.GameId = g.Id
GROUP BY g.Title
ORDER BY HoursPlayed DESC
");

foreach (var g in topGames)
{
    Console.WriteLine($"{g.Title} - {g.HoursPlayed:F2} hours");
}

var ranking = connection.Query(@"
SELECT m.FullName,
SUM(s.DurationMinutes) AS TotalMinutes
FROM Sessions s
JOIN Members m ON s.MemberId = m.Id
GROUP BY m.FullName
ORDER BY TotalMinutes DESC
");

foreach (var r in ranking)
{
    Console.WriteLine($"{r.FullName} - {r.TotalMinutes} minutes");
}

var stats = connection.QueryFirst(@"
SELECT 
COUNT(*) AS TotalSessions,
SUM(DurationMinutes) AS TotalMinutes
FROM Sessions
");

Console.WriteLine($"Sessions: {stats.TotalSessions}");
Console.WriteLine($"Total minutes: {stats.TotalMinutes}");

var stats = connection.QueryFirst(@"
SELECT 
COUNT(*) AS TotalSessions,
SUM(DurationMinutes) AS TotalMinutes
FROM Sessions
");

Console.WriteLine($"Sessions: {stats.TotalSessions}");
Console.WriteLine($"Total minutes: {stats.TotalMinutes}");