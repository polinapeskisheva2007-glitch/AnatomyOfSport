using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AnatomyOfSport.Models;

namespace AnatomyOfSport
{
    public partial class App : System.Windows.Application
    {
        // WinAPI для HTML Help
        [DllImport("hhctrl.ocx", CharSet = CharSet.Auto)]
        private static extern IntPtr HtmlHelp(
            IntPtr hwndCaller,
            string pszFile,
            uint uCommand,
            uint dwData
        );

        private const uint HH_HELP_CONTEXT = 0x000F;
        private const uint HH_DISPLAY_TOC = 0x0001;

        // Текущая тренировка
        public static List<WorkoutExercise> CurrentWorkout = new();

        // История тренировок
        public static List<WorkoutRecord> WorkoutHistory = new();

        // Проверить есть ли упражнение в тренировке
        public static bool IsExerciseInWorkout(string name)
        {
            return CurrentWorkout.Exists(e => e.ExerciseName == name);
        }

        // Добавить упражнение в тренировку
        public static void AddToWorkout(string name)
        {
            if (!IsExerciseInWorkout(name))
            {
                CurrentWorkout.Add(new WorkoutExercise
                {
                    ExerciseName = name,
                    Sets = 3,
                    Reps = 12
                });
            }
        }

        // Открыть справку на конкретном разделе по ID из help.h
        public static void ShowHelp(int topicId)
        {
            string helpFile = System.IO.Path.GetFullPath("анатомия спорта.chm");
            try
            {
                string hhctrl = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.System),
                    "hhctrl.ocx");

                HtmlHelp(IntPtr.Zero, helpFile, HH_HELP_CONTEXT, (uint)topicId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Help error: {ex.Message}");
                HtmlHelp(IntPtr.Zero, helpFile, HH_DISPLAY_TOC, 0);
            }
        }
    }
}