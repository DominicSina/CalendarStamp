using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CalendarStamp.ViewModels
{
    using CalendarStamp.Models;

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

        private WorkEvent _lastEvent;
        public WorkEvent lastEvent
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
                WorkEvent e = new WorkEvent();
                
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
                lastEvent.end();

                await CrossCalendars.Current.AddOrUpdateEventAsync(c.First(), lastEvent);

                lastEvent = null;
                lastEventSet = false;

                await _dialogService.DisplayAlertAsync("CalendarStamp", "The last work event has been ended", "OK");
            }
        }
    }
}
