using MythApi.Common.Database.Models;

namespace MythApi.Common.Database;

public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private bool _initialized = false;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void InitializeDatabase()
    {
        if (_initialized)
            return;
            
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Check if we already have data
        if (!dbContext.Mythologies.Any())
        {
            // Create Norse mythology
            var norseMythology = new Mythology
            {
                Name = "Norse",
                Gods = new List<God>
                {
                    new God
                    {
                        Name = "Odin",
                        Description = "Odin is the chief god in Norse mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Woden" },
                            new Alias { Name = "Wotan" },
                            new Alias { Name = "Allfather" },
                            new Alias { Name = "Ygg" },
                            new Alias { Name = "Bolverk" },
                            new Alias { Name = "Grimnir" },
                            new Alias { Name = "Oski" },
                            new Alias { Name = "Omi" },
                            new Alias { Name = "Biflindi" },
                            new Alias { Name = "Sigfodr" },
                            new Alias { Name = "Hnikudr" },
                            new Alias { Name = "Harbard" }
                        }
                    },
                    new God
                    {
                        Name = "Thor",
                        Description = "Thor is the god of thunder in Norse mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Donar" },
                            new Alias { Name = "Thunor" }
                        }
                    },
                    new God
                    {
                        Name = "Loki",
                        Description = "Loki is the god of mischief in Norse mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Lopt" }
                        }
                    },
                    new God
                    {
                        Name = "Frigg",
                        Description = "Frigg is the goddess of marriage in Norse mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Frigga" }
                        }
                    },
                    new God
                    {
                        Name = "Baldur",
                        Description = "Baldur is the god of light in Norse mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Balder" }
                        }
                    },
                    new God
                    {
                        Name = "Tyr",
                        Description = "Tyr is the god of war in Norse mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Tiw" }
                        }
                    }
                }
            };
            dbContext.Mythologies.Add(norseMythology);
            
            // Create Greek mythology
            var greekMythology = new Mythology
            {
                Name = "Greek",
                Gods = new List<God>
                {
                    new God
                    {
                        Name = "Zeus",
                        Description = "Zeus is the chief god in Greek mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Jupiter" }
                        }
                    },
                    new God
                    {
                        Name = "Poseidon",
                        Description = "Poseidon is the god of the sea in Greek mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Neptune" }
                        }
                    },
                    new God
                    {
                        Name = "Hades",
                        Description = "Hades is the god of the underworld in Greek mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Pluto" }
                        }
                    },
                    new God
                    {
                        Name = "Athena",
                        Description = "Athena is the goddess of wisdom in Greek mythology."
                    },
                    new God
                    {
                        Name = "Apollo",
                        Description = "Apollo is the god of the sun in Greek mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Phoebus" }
                        }
                    },
                    new God
                    {
                        Name = "Artemis",
                        Description = "Artemis is the goddess of the hunt in Greek mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Diana" }
                        }
                    },
                    new God
                    {
                        Name = "Ares",
                        Description = "Ares is the god of war in Greek mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Mars" }
                        }
                    }
                }
            };
            dbContext.Mythologies.Add(greekMythology);
            
            // Create Roman mythology
            var romanMythology = new Mythology
            {
                Name = "Roman",
                Gods = new List<God>
                {
                    new God
                    {
                        Name = "Jupiter",
                        Description = "Jupiter is the chief god in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Zeus" }
                        }
                    },
                    new God
                    {
                        Name = "Neptune",
                        Description = "Neptune is the god of the sea in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Poseidon" }
                        }
                    },
                    new God
                    {
                        Name = "Pluto",
                        Description = "Pluto is the god of the underworld in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Hades" }
                        }
                    },
                    new God
                    {
                        Name = "Minerva",
                        Description = "Minerva is the goddess of wisdom in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Athena" }
                        }
                    },
                    new God
                    {
                        Name = "Apollo",
                        Description = "Apollo is the god of the sun in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Phoebus" }
                        }
                    },
                    new God
                    {
                        Name = "Diana",
                        Description = "Diana is the goddess of the hunt in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Artemis" }
                        }
                    },
                    new God
                    {
                        Name = "Mars",
                        Description = "Mars is the god of war in Roman mythology.",
                        Aliases = new List<Alias>
                        {
                            new Alias { Name = "Ares" }
                        }
                    }
                }
            };
            dbContext.Mythologies.Add(romanMythology);

            // Save changes
            dbContext.SaveChanges();
            
            Console.WriteLine("Database initialized with Norse and Greek mythologies");
        }
        _initialized = true;
    }
}
