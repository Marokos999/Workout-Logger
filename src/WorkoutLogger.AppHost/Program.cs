var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();

var db = postgres.AddDatabase("workoutdb");


builder.AddProject<Projects.WorkoutLogger_API>("api")
       .WithReference(db)
       .WaitFor(db);


builder.Build().Run();