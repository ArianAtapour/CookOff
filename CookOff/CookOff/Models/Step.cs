using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;

namespace CookOff.Models
{
    public class Step : INotifyPropertyChanged
    {
        //Fields
        private bool isSelected;
        private TimeSpan timer;
        private bool isTimerRunning;
        private bool isTimerPaused;
        private TimeSpan originalTimer;

        //Methods
        public int RecipeID { get; set; }
        public string Description { get; set; }
        public bool TimerRequired { get; set; }
        public string StepNumber { get; set; }
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

        //Timer constructor
        public Step()
        {
            StartTimerCommand = new Command(StartTimer);
            PauseTimerCommand = new Command(PauseTimer);
            StopTimerCommand = new Command(StopTimer);
        }

        //Main constructor
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

        //Start timer function
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

        //Pausing the timer
        private void PauseTimer()
        {
            isTimerPaused = true;
            isTimerRunning = false;
        }

        //Stopping the timer (resets the timer to its original time)
        private void StopTimer()
        {
            isTimerRunning = false;
            isTimerPaused = false;
            Timer = originalTimer; // Reset the timer to the original time
        }

        //Play an .mp3 file function
        private async void PlaySound()
        {
            try
            {
                Debug.WriteLine("Attempting to play sound in PlaySound method");

                // Directly create the audio player and play the sound
                var audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("Resources/Raw/timer_end.mp3"));

                if (audioPlayer == null)
                {
                    Debug.WriteLine("AudioPlayer creation failed");
                    return;
                }

                audioPlayer.Play();
                Debug.WriteLine("Sound played");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in PlaySound method: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
