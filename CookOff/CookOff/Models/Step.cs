using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Plugin.SimpleAudioPlayer;

namespace CookOff.Models
{
    public class Step : INotifyPropertyChanged
    {
        private bool isSelected;
        private TimeSpan timer;
        private bool isTimerRunning;
        private bool isTimerPaused;
        private TimeSpan originalTimer;

        public int RecipeID { get; set; }
        public string Description { get; set; }
        public bool TimerRequired { get; set; }
        public TimeSpan Timer
        {
            get => timer;
            set
            {
                timer = value;
                OnPropertyChanged();
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartTimerCommand { get; }
        public ICommand PauseTimerCommand { get; }
        public ICommand StopTimerCommand { get; }

        public Step()
        {
            StartTimerCommand = new Command(StartTimer);
            PauseTimerCommand = new Command(PauseTimer);
            StopTimerCommand = new Command(StopTimer);
        }

        public Step(int recipeID, string description, bool timerRequired, TimeSpan timer)
        {
            RecipeID = recipeID;
            Description = description;
            TimerRequired = timerRequired;
            Timer = timer;

            StartTimerCommand = new Command(StartTimer);
            PauseTimerCommand = new Command(PauseTimer);
            StopTimerCommand = new Command(StopTimer);
        }

        private void StartTimer()
        {
            if (!isTimerRunning || isTimerPaused)
            {
                isTimerRunning = true;
                isTimerPaused = false;
                originalTimer = Timer; // Store the original time when the timer starts
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (!isTimerRunning)
                        return false;

                    Timer = Timer.Subtract(TimeSpan.FromSeconds(1));
                    if (Timer <= TimeSpan.Zero)
                    {
                        Timer = TimeSpan.Zero;
                        isTimerRunning = false;
                        // Play sound
                        PlaySound();
                    }
                    return isTimerRunning;
                });
            }
        }

        private void PauseTimer()
        {
            isTimerPaused = true;
            isTimerRunning = false;
        }

        private void StopTimer()
        {
            isTimerRunning = false;
            isTimerPaused = false;
            Timer = originalTimer; // Reset the timer to the original time
        }

        private void PlaySound()
        {
            var player = CrossSimpleAudioPlayer.Current;
            player.Load("Resources/Raw/timer_end.mp3");
            player.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
