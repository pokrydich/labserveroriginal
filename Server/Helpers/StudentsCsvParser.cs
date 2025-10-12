namespace LabServer.Server.Helpers;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using CsvHelper;
using CsvHelper.Configuration;

/// <summary>
/// Student information from CSV record
/// </summary>
public class StudentCsvRecord
{
    public System.String Name { get; set; } = System.String.Empty;
    public System.String Email { get; set; } = System.String.Empty;
}

/// <summary>
/// Class for parsing type agnostic two column CSV files
/// </summary>
public class TwoColumnRecord
{
    public System.String FirstColumn { get; set; } = System.String.Empty;
    public System.String SecondColumn { get; set; } = System.String.Empty;
}

/// <summary>
/// Deocrator for CsvParser module
/// </summary>
public static class StudentsCsvParser
{
    private static readonly Regex cEmailRegex = new Regex(@"[\w-\.]+@([\w-]+\.)+[\w-]{2,4}");
    /// <summary>
    /// Tries to guess CSV delimiter by looking for email address in CSV input
    /// </summary>
    /// <param name="rawInput"></param>
    /// <returns></returns>
    public static System.String? GuessDelimiter(System.String rawInput)
    {
        var match = cEmailRegex.Match(rawInput);
        return match.Index == 0
            ? (match.Index + match.Length + 1 >= rawInput.Length 
                ? null // matched full string (not a valid CSV - only email address)
                : rawInput.Substring(match.Index + match.Length, 1) )
            : rawInput.Substring(match.Index - 1, 1);
    }
    /// <summary>
    /// Indicates if first CSV column is the column with email addresses
    /// </summary>
    /// <param name="records">list of parsed CSV records</param>
    /// <returns>true if 1st column contains e-mails, false if 2nd contains e-mails</returns>
    /// <exception cref="NotImplementedException">thrown if e-mail column was not identified</exception>
    private static System.Boolean IsFirstColumnEmail(List<TwoColumnRecord> records)
    {
        foreach (var record in records)
        {
            if (cEmailRegex.IsMatch(record.FirstColumn))
            {
                return true;
            }
            else if (cEmailRegex.IsMatch(record.SecondColumn))
            {
                return false;
            }
        }

        throw new NotImplementedException("Couldn't determing email column in students CSV-file");
    }
    /// <summary>
    /// Parses CSV file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rawInput"></param>
    /// <param name="expectHeader"></param>
    /// <returns></returns>
    public static List<StudentCsvRecord> Parse(System.String rawInput)
    {
        var delimiter = GuessDelimiter(rawInput);

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        if (delimiter != null)
        {
            configuration.Delimiter = delimiter;
        }

        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(rawInput)))
        {
            using (TextReader sr = new StreamReader(ms))
            {
                var csvReader = new CsvReader(sr, configuration);
                var records = csvReader.GetRecords<TwoColumnRecord>().ToList();

                var isFirstColumnEmail = IsFirstColumnEmail(records);
                return records.Select(r => new StudentCsvRecord
                                        {
                                            Email = isFirstColumnEmail ? r.FirstColumn : r.SecondColumn,
                                            Name = isFirstColumnEmail ? r.SecondColumn : r.FirstColumn
                                        }).ToList();
            }
        }
    }
}