using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using WebApiFunction.Configuration;

namespace HelixTicket.InternalModels
{

    public class TicketIdentifierCheckResponse
    {
        private Match _result = null;
        private string _datePartOfTicketIdentifier = null;

        public string TicketIdentifierFully
        {
            get
            {
                if (_result != null)
                    return _result.Value;

                return null;
            }
        }
        public bool IsValidTicketIdentifier { get => _result == null ? false : _result.Success; }
        public string TicketIdentifierSourceFully { get; private set; } = null;
        public int Year
        {
            get
            {
                if (TicketIdentifierFully != null)
                {
                    Match yearPartRegExMatch = Regex.Match(TicketIdentifierFully, Handler.TicketHandler.TicketIdentifierDatePartExtractRegExPattern);
                    if (yearPartRegExMatch.Success)
                    {
                        _datePartOfTicketIdentifier = yearPartRegExMatch.Value;
                        if (int.TryParse(_datePartOfTicketIdentifier, out int year))
                        {
                            return year;
                        }
                    }
                }
                return GeneralDefs.NotFoundResponseValue;
            }
        }
        public int Month
        {
            get
            {
                if (_datePartOfTicketIdentifier != null)
                {
                    string monthPart = _datePartOfTicketIdentifier.Substring(4, 2);
                    if (int.TryParse(monthPart, out int month))
                    {
                        return month;
                    }
                }
                return GeneralDefs.NotFoundResponseValue;
            }
        }
        public int Day
        {
            get
            {
                if (_datePartOfTicketIdentifier != null)
                {
                    string dayPart = _datePartOfTicketIdentifier.Substring(6, 2);
                    if (int.TryParse(dayPart, out int day))
                    {
                        return day;
                    }
                }
                return GeneralDefs.NotFoundResponseValue;
            }
        }
        public int CalendarWeek
        {
            get
            {
                if (Day != GeneralDefs.NotFoundResponseValue && Month != GeneralDefs.NotFoundResponseValue && Year != GeneralDefs.NotFoundResponseValue)
                {
                    DateTime dateTime = new DateTime(Year, Month, Day);
                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                    Calendar calendar = cultureInfo.Calendar;
                    CalendarWeekRule calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
                    DayOfWeek dayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
                    return calendar.GetWeekOfYear(dateTime, calendarWeekRule, dayOfWeek);
                }
                return GeneralDefs.NotFoundResponseValue;
            }
        }
        public int IncrementCounter
        {
            get
            {
                if (TicketIdentifierFully != null)
                {
                    Match match = Regex.Match(TicketIdentifierFully, Handler.TicketHandler.TicketIdentifierIncrementCounterExtractRegExPattern);
                    if (match.Success)
                    {
                        string incrementPart = match.Value.Replace("#", "").Replace("]", "");
                        if (int.TryParse(incrementPart, out int counter))
                        {
                            return counter;
                        }
                    }
                }
                return GeneralDefs.NotFoundResponseValue;
            }
        }

        public TicketIdentifierCheckResponse(string input)
        {
            _result = Regex.Match(input,Handler.TicketHandler.TicketIdentifierRegExPattern);
            TicketIdentifierSourceFully = input;
        }

    }
}
