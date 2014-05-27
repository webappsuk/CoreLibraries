using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Jeeves.Models;
using WebApplications.Utilities;
using WebApplications.Utilities.Logging;

namespace Jeeves.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        private readonly Connection _connection;
        private DateTime _eventsStart;
        private DateTime _eventsEnd;
        private int _minimumZoomRange;
        private readonly ObservableCollection<Event> _events = new ObservableCollection<Event>();
        private SelectionMode _timelineSelectionMode = SelectionMode.Extended;
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime _visibleStartDate;
        private DateTime _visibleEndDate;

        public Connection Connection
        {
            get { return _connection; }
        }

        public string Name
        {
            get { return _connection.Name; }
            set
            {
                if (value == _connection.Name) return;
                _connection.Name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Event> Events
        {
            get { return _events; }
        }

        public SelectionMode TimelineSelectionMode
        {
            get { return _timelineSelectionMode; }
            private set
            {
                if (value == _timelineSelectionMode) return;
                _timelineSelectionMode = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                DateTime startDate = _startDate;
                if (startDate == value) return;

                _startDate = value;
                OnPropertyChanged();
                if (_visibleStartDate == startDate)
                    VisibleStartDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                DateTime endDate = _endDate;
                if (endDate == value) return;

                _endDate = value;
                OnPropertyChanged();
                if (_visibleEndDate == endDate)
                    VisibleEndDate = value;
            }
        }

        public DateTime VisibleStartDate
        {
            get
            {
                return _visibleStartDate;
            }
            set
            {
                _visibleStartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime VisibleEndDate
        {
            get
            {
                return _visibleEndDate;
            }
            set
            {
                _visibleEndDate = value;
                OnPropertyChanged();
            }
        }

        public ConnectionViewModel(Connection connection)
        {
            if (connection == null)
            {
                /*if (!IsInDesignMode)
                    throw new InvalidOperationException("This constructor is only intended for use at design time.");*/
                connection = new Connection("Test Service", @"\\.\pipe\Test_Service");
            }
            _connection = connection;
            EndDate = DateTime.Now;
            StartDate = EndDate - TimeSpan.FromMinutes(1);
            VisibleEndDate = EndDate;
            VisibleStartDate = StartDate;

            Random r = new Random();
            for (int a = 0; a < 100; a++)
                AddEvent(new LogEvent((a % 2 == 0),
                    new Log(
                        new Dictionary<string, string>
                        {
                            {Log.GuidKey, CombGuid.NewCombGuid(DateTime.Now - TimeSpan.FromMilliseconds(r.Next(0, 1000000))).ToString("D")},
                            {Log.LevelKey, ((LoggingLevel) (int) Math.Pow(2, (a%8))).ToString()},
                            {Log.MessageFormatKey, "Test log message " + (a + 1)}
                        })));
        }

        public void AddEvent(Event @event)
        {
            _events.Add(@event);
            if (@event.StartDate < StartDate) StartDate = @event.StartDate;
            if (@event.EndDate > EndDate) EndDate = @event.EndDate;
        }
    }
}