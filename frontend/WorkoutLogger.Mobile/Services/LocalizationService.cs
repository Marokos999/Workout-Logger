using System.ComponentModel;

namespace WorkoutLogger.Mobile.Services;

public sealed class LocalizationService : INotifyPropertyChanged
{
    public static readonly LocalizationService Instance = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _lang = "sr";

    public bool IsEnglish => _lang == "en";

    public string this[string key] =>
        _strings.TryGetValue(_lang, out var dict) && dict.TryGetValue(key, out var val) ? val : key;

    public void SetLanguage(string lang)
    {
        if (_lang == lang) return;
        _lang = lang;
        Preferences.Set("app_language", lang);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnglish)));
    }

    private LocalizationService()
    {
        _lang = Preferences.Get("app_language", "sr");
    }

    private static readonly Dictionary<string, Dictionary<string, string>> _strings = new()
    {
        ["sr"] = new()
        {
            // Auth
            ["AppTitle"]           = "Workout Logger",
            ["Email"]              = "Email",
            ["Password"]           = "Lozinka",
            ["Login"]              = "Prijavi se",
            ["NoAccount"]          = "Nemate nalog? Registrujte se",
            ["RegisterTitle"]      = "Registracija",
            ["Username"]           = "Korisničko ime",
            ["Register"]           = "Registruj se",
            // Workouts
            ["PageWorkouts"]       = "Moji treninzi",
            ["EmptyWorkouts"]      = "Nema treninga još uvek.",
            ["EmptyWorkoutsHint"]  = "Klikni + da kreiraš prvi trening.",
            ["LoadMore"]           = "Učitaj još",
            ["PageNewWorkout"]     = "Novi trening",
            ["WorkoutName"]        = "Naziv treninga",
            ["Notes"]              = "Beleške (opciono)",
            ["CreateWorkout"]      = "Kreiraj trening",
            // Sets / Detail
            ["PageAddSet"]         = "Dodaj set",
            ["ExerciseLabel"]      = "Vežba",
            ["SelectExercise"]     = "Odaberi vežbu",
            ["SetNumber"]          = "Broj seta",
            ["Reps"]               = "Ponavljanja",
            ["Weight"]             = "Težina (kg)",
            ["NoteOptional"]       = "Beleška (opciono)",
            ["SaveSet"]            = "Sačuvaj set",
            ["NoSets"]             = "Nema setova. Dodaj prvi set!",
            ["EndWorkout"]         = "Završi trening",
            ["Rest"]               = "⏱ Odmor",
            ["TimerDone"]          = "✓ Gotovo!",
            ["AddSet"]             = "+ Set",
            // Summary
            ["PageSummary"]        = "Trening završen",
            ["GreatJob"]           = "Odlično odrađeno!",
            ["Duration"]           = "Trajanje",
            ["Sets"]               = "Serije",
            ["Volume"]             = "Volumen",
            ["ByExercises"]        = "Po vežbama",
            ["MaxWeightLabel"]     = "max težina",
            ["Finish"]             = "Završi",
            // Exercises
            ["PageExercises"]      = "Vežbe",
            ["SearchExercises"]    = "Pretraži vežbe...",
            ["AddExercise"]        = "+ Dodaj",
            ["MuscleGroup"]        = "Mišićna grupa:",
            ["Equipment"]          = "Oprema:",
            ["PersonalRecord"]     = "Lični rekord",
            ["ProgressHistory"]    = "Istorija napretka",
            ["NoDataYet"]          = "Nema podataka još uvek.",
            ["NoExerciseYet"]      = "Još nisi radio ovu vežbu.",
            ["AddToWorkout"]       = "Dodaj je u trening da vidiš napredak.",
            ["PageNewExercise"]    = "Nova vežba",
            ["ExerciseName"]       = "Naziv vežbe",
            ["ExerciseNameHint"]   = "npr. Bench Press",
            ["MuscleGroupForm"]    = "Mišićna grupa",
            ["EquipmentForm"]      = "Oprema",
            ["SaveExercise"]       = "Sačuvaj vežbu",
            // Progress
            ["PageProgress"]       = "Napredak",
            ["Logout"]             = "Odjavi se",
            ["VolumeByExercise"]   = "Ukupni volumen po vežbi",
            ["ProgressByExercise"] = "Napredak po vežbi",
            ["SelectExPicker"]     = "Izaberi vežbu",
            ["PersonalRecords"]    = "Personalni rekordi",
            ["NoRecords"]          = "Nema podataka.",
            // Profile
            ["PageProfile"]        = "Profil",
            ["UsernameLabel"]      = "Korisničko ime",
            ["EmailLabel"]         = "Email",
            ["LogoutButton"]       = "Odjavi se",
            ["Language"]           = "Jezik",
            // Shell tabs
            ["TabWorkouts"]        = "Treninzi",
            ["TabExercises"]       = "Vežbe",
            ["TabProgress"]        = "Napredak",
            ["TabProfile"]         = "Profil",
        },
        ["en"] = new()
        {
            // Auth
            ["AppTitle"]           = "Workout Logger",
            ["Email"]              = "Email",
            ["Password"]           = "Password",
            ["Login"]              = "Sign in",
            ["NoAccount"]          = "No account? Register",
            ["RegisterTitle"]      = "Register",
            ["Username"]           = "Username",
            ["Register"]           = "Register",
            // Workouts
            ["PageWorkouts"]       = "My Workouts",
            ["EmptyWorkouts"]      = "No workouts yet.",
            ["EmptyWorkoutsHint"]  = "Tap + to create your first workout.",
            ["LoadMore"]           = "Load more",
            ["PageNewWorkout"]     = "New Workout",
            ["WorkoutName"]        = "Workout name",
            ["Notes"]              = "Notes (optional)",
            ["CreateWorkout"]      = "Create workout",
            // Sets / Detail
            ["PageAddSet"]         = "Add set",
            ["ExerciseLabel"]      = "Exercise",
            ["SelectExercise"]     = "Select exercise",
            ["SetNumber"]          = "Set number",
            ["Reps"]               = "Reps",
            ["Weight"]             = "Weight (kg)",
            ["NoteOptional"]       = "Note (optional)",
            ["SaveSet"]            = "Save set",
            ["NoSets"]             = "No sets. Add your first set!",
            ["EndWorkout"]         = "Finish workout",
            ["Rest"]               = "⏱ Rest",
            ["TimerDone"]          = "✓ Done!",
            ["AddSet"]             = "+ Set",
            // Summary
            ["PageSummary"]        = "Workout complete",
            ["GreatJob"]           = "Great job!",
            ["Duration"]           = "Duration",
            ["Sets"]               = "Sets",
            ["Volume"]             = "Volume",
            ["ByExercises"]        = "By exercise",
            ["MaxWeightLabel"]     = "max weight",
            ["Finish"]             = "Finish",
            // Exercises
            ["PageExercises"]      = "Exercises",
            ["SearchExercises"]    = "Search exercises...",
            ["AddExercise"]        = "+ Add",
            ["MuscleGroup"]        = "Muscle group:",
            ["Equipment"]          = "Equipment:",
            ["PersonalRecord"]     = "Personal record",
            ["ProgressHistory"]    = "Progress history",
            ["NoDataYet"]          = "No data yet.",
            ["NoExerciseYet"]      = "You haven't trained this exercise yet.",
            ["AddToWorkout"]       = "Add it to a workout to see your progress.",
            ["PageNewExercise"]    = "New exercise",
            ["ExerciseName"]       = "Exercise name",
            ["ExerciseNameHint"]   = "e.g. Bench Press",
            ["MuscleGroupForm"]    = "Muscle group",
            ["EquipmentForm"]      = "Equipment",
            ["SaveExercise"]       = "Save exercise",
            // Progress
            ["PageProgress"]       = "Progress",
            ["Logout"]             = "Sign out",
            ["VolumeByExercise"]   = "Total volume by exercise",
            ["ProgressByExercise"] = "Progress by exercise",
            ["SelectExPicker"]     = "Select exercise",
            ["PersonalRecords"]    = "Personal records",
            ["NoRecords"]          = "No data.",
            // Profile
            ["PageProfile"]        = "Profile",
            ["UsernameLabel"]      = "Username",
            ["EmailLabel"]         = "Email",
            ["LogoutButton"]       = "Sign out",
            ["Language"]           = "Language",
            // Shell tabs
            ["TabWorkouts"]        = "Workouts",
            ["TabExercises"]       = "Exercises",
            ["TabProgress"]        = "Progress",
            ["TabProfile"]         = "Profile",
        }
    };
}
