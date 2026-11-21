using System.Collections.Generic;

namespace DanceLearning.Models
{
    /// <summary>
    /// Provides sample/demo data for the application.
    /// In production, replace with database or API service.
    /// </summary>
    public static class AppData
    {
        public static List<DanceStyle> GetDanceStyles()
        {
            return new List<DanceStyle>
            {
                new DanceStyle
                {
                    Name = "Hip-Hop",
                    Description = "Уличный стиль с мощной энергетикой и выразительной пластикой",
                    Difficulty = "Средний",
                    LessonsCount = 12,
                    ImagePath = "Assets/Images/hiphop.png",
                    AccentColor = "#FF6C3CE1",
                    Lessons = GetSampleLessons("Hip-Hop")
                },
                new DanceStyle
                {
                    Name = "Breakdance",
                    Description = "Акробатические элементы, вращения на полу и заморозки",
                    Difficulty = "Сложный",
                    LessonsCount = 8,
                    ImagePath = "Assets/Images/breakdance.png",
                    AccentColor = "#FFEC4899",
                    Lessons = GetSampleLessons("Breakdance")
                },
                new DanceStyle
                {
                    Name = "K-Pop Cover Dance",
                    Description = "Точные хореографии из клипов корейских исполнителей",
                    Difficulty = "Начинающий",
                    LessonsCount = 15,
                    ImagePath = "Assets/Images/kpop.png",
                    AccentColor = "#FF06B6D4",
                    Lessons = GetSampleLessons("K-Pop")
                },
                new DanceStyle
                {
                    Name = "High Heels",
                    Description = "Женственный стиль на каблуках: sensual движения и провокация",
                    Difficulty = "Средний",
                    LessonsCount = 10,
                    ImagePath = "Assets/Images/highheels.png",
                    AccentColor = "#FFF59E0B",
                    Lessons = GetSampleLessons("High Heels")
                }
            };
        }

        private static List<Lesson> GetSampleLessons(string styleName)
        {
            return new List<Lesson>
            {
                new Lesson
                {
                    Number = 1,
                    Title = $"{styleName} — Основы и стойка",
                    VideoPath = "",
                    IsCompleted = true,
                    Steps = GetSampleSteps()
                },
                new Lesson
                {
                    Number = 2,
                    Title = $"{styleName} — Базовые движения",
                    VideoPath = "",
                    Steps = GetSampleSteps()
                },
                new Lesson
                {
                    Number = 3,
                    Title = $"{styleName} — Первая связка",
                    VideoPath = "",
                    Steps = GetSampleSteps()
                }
            };
        }

        private static List<DanceStep> GetSampleSteps()
        {
            return new List<DanceStep>
            {
                new DanceStep { Number = 1, Name = "Вход в позицию", Description = "Начальная стойка с правой ноги" },
                new DanceStep { Number = 2, Name = "Перенос веса", Description = "Перенос центра тяжести влево" },
                new DanceStep { Number = 3, Name = "Движение руками", Description = "Волна от плеча к запястью" },
                new DanceStep { Number = 4, Name = "Шаг назад", Description = "Скользящий шаг назад на носке" },
                new DanceStep { Number = 5, Name = "Финальная поза", Description = "Фиксация в конечной позиции" }
            };
        }

        public static UserProfile GetUserProfile() => new UserProfile
        {
            Name = "Алексей",
            TotalPoints = 1250,
            LessonsCompleted = 7,
            BundlesLearned = 3,
            TrainingStreak = 5,
            OverallProgress = 0.42
        };
    }
}
