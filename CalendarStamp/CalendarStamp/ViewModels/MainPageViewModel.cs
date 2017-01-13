using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CalendarStamp.ViewModels
{
    using Plugin.Calendars;
    using Plugin.Calendars.Abstractions;

    using Prism.Services;

    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private IPageDialogService _dialogService;

        private bool _lastEventSet;
        public bool lastEventSet
        {
            get { return _lastEventSet; }
            set { SetProperty(ref _lastEventSet, value); }
        }

        private CalendarEvent _lastEvent;
        public CalendarEvent lastEvent
        {
            get { return _lastEvent; }
            set { SetProperty(ref _lastEvent, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand NewStartEventCommand { get; }
        public DelegateCommand NewEndEventCommand { get; }


        public MainPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;

            NewStartEventCommand = new DelegateCommand(newStartEvent);
            NewEndEventCommand = new DelegateCommand(newEndEvent);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"] + " and Prism";
        }

        private async void newStartEvent()
        {
            if (lastEventSet)
                await _dialogService.DisplayAlertAsync("CalendarStamp", "Please end your last event first", "OK");
            else
            {
                IList<Calendar> c = await CrossCalendars.Current.GetCalendarsAsync();
                CalendarEvent e = new CalendarEvent();
                e.AllDay = false;
                e.Description = "Work event. Currently not finished";
                e.Name = "Work";
                e.Start = DateTime.Now;
                e.End = DateTime.Now;

                lastEvent = e;
                lastEventSet = true;

                await CrossCalendars.Current.AddOrUpdateEventAsync(c.First(), e);

                await _dialogService.DisplayAlertAsync("CalendarStamp", "New work event has been created", "OK");
            }
        }

        private async void newEndEvent()
        {
            if (!lastEventSet)
                await _dialogService.DisplayAlertAsync("CalendarStamp", "Please start a new event first", "OK");
            else
            {
                IList<Calendar> c = await CrossCalendars.Current.GetCalendarsAsync();
                lastEvent.End = DateTime.Now;
                lastEvent.Description = "Finished work event.";
                await CrossCalendars.Current.AddOrUpdateEventAsync(c.First(), lastEvent);

                lastEvent = null;
                lastEventSet = false;

                await _dialogService.DisplayAlertAsync("CalendarStamp", "The last work event has been ended", "OK");
            }
        }
    }
}
