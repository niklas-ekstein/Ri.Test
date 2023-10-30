using Ri.Interview.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ri.Interview.Interfaces;
using Ri.Interview.Models;

namespace Ri.Interview
{
    public class ApplicationRunner
    {
        private readonly IAccountService _accountService;
        private readonly IConsoleWrapperService _console;
        private readonly ILoginService _loginService;
        private readonly IProjectService _projectService;
        private readonly ITeamService _teamService;
        private readonly Dictionary<ConsoleKey, Func<Task>> _menuActions;
        private string jwtToken;
        private bool isLoggedIn = false;

        public ApplicationRunner(
            IConsoleWrapperService console,
            IAccountService accountService,
            ILoginService loginService,
            IProjectService projectService,
            ITeamService teamService)
        {
            _accountService = accountService;
            _loginService = loginService;
            _projectService = projectService;
            _teamService = teamService;
            _console = console;
            
            // You can press 1 and 2 even if your at another menu.. wtf fix..
            _menuActions = new Dictionary<ConsoleKey, Func<Task>>
            {
                { ConsoleKey.NumPad1, CreateAccount },
                { ConsoleKey.D1, CreateAccount },
                { ConsoleKey.NumPad2, Login },
                { ConsoleKey.D2, Login },
                { ConsoleKey.NumPad3, CreateProject },
                { ConsoleKey.D3, CreateProject },
                { ConsoleKey.NumPad4, UpdateProject },
                { ConsoleKey.D4, UpdateProject },
                { ConsoleKey.NumPad5, GetAllProjects },
                { ConsoleKey.D5, GetAllProjects },
                { ConsoleKey.Q, async () => QuitApplication() }
            };
        }

        public async Task Run()
        {
            var shouldRun = true;
            while (shouldRun)
            {
                DisplayOptions();
                _console.Write("Enter an option: ");
                var input = _console.ReadKey();
                _console.WriteLine("\n");

                if (_menuActions.ContainsKey(input.Key))
                {
                    await _menuActions[input.Key].Invoke();
                    if (input.Key == ConsoleKey.Q)
                    {
                        shouldRun = false;
                    }
                }
                else
                {
                    _console.WriteLine("Invalid option!");
                }
            }

            _console.Write("\n\rPress any key to exit!");
            _console.ReadKey();
        }

        public void DisplayOptions()
        {
            if (isLoggedIn)
            {
                _console.WriteLine("Choose an option:");
                _console.WriteLine("3 - Create a new project");
                _console.WriteLine("4 - Update project");
                _console.WriteLine("5 - Get all projects");
                _console.WriteLine("q - Quit");
            }
            else
            {
                _console.WriteLine("Choose an option:");
                _console.WriteLine("1 - Create account");
                _console.WriteLine("2 - Login");
            }
        }

        private async Task CreateAccount()
        {
            _console.WriteLine("==== Create Account ====");

            _console.Write("Enter name: ");
            var name = _console.ReadLine();

            _console.Write("Enter email: ");
            var email = _console.ReadLine();

            _console.Write("Enter password: ");
            var password = _console.ReadLine();

            var account = new Account(name, email, password);
            
            bool success = await _accountService.RegisterAsync(account);

            if (success)
            {
                _console.WriteLine("Account created successfully!");
            }
            else
            {
                _console.WriteLine("Failed to create the account. Please try again.");
            }
        }
        
        private async Task Login()
        {
            _console.WriteLine("==== Login ====");

            _console.Write("Enter email: ");
            var email = "niklas.ekstein@gmail.com"; //_console.ReadLine();

            _console.Write("Enter password: ");
            var password = "jagdu123456"; //_console.ReadLine();

            var (success, token) = await _loginService.LoginAsync(email, password);

            if (success)
            {
                Console.WriteLine("Token: " + token);
                isLoggedIn = true;
                jwtToken = token;
                _console.WriteLine("Successfully logged in!");
            }
            else
            {
                _console.WriteLine("Failed to login. Please try again.");
            }
        }
        
        private async Task CreateProject()
        {
            _console.WriteLine("==== Create Project ====");

            _console.Write("Enter title: ");
            var title = _console.ReadLine();

            _console.Write("Enter name: ");
            var name = _console.ReadLine();

            _console.Write("Enter description: ");
            var description = _console.ReadLine();

            var project = new Project
            {
                Title = title,
                Name = name,
                Description = description,
                Template = "https://raw.githubusercontent.com/formio/formio-app-todo/master/src/project.json",
                Settings = new Settings { Cors = "*" },
            };

            bool success = await _projectService.CreateProjectAsync(jwtToken, project);

            if (success)
            {
                _console.WriteLine("Project created successfully!");
            }
            else
            {
                _console.WriteLine("Failed to create the project. Please try again.");
            }
        }
        
        private async Task GetAllProjects()
        {
            _console.WriteLine("==== List of All Projects ====");

            var projects = await _projectService.GetAllProjectsAsync(jwtToken);
            if (projects != null)
            {
                foreach (var project in projects)
                {
                    _console.WriteLine($"ID: {project.Id}, Title: {project.Title}, Name: {project.Name}, Description: {project.Description}");
                }
            }
            else
            {
                _console.WriteLine("Failed to fetch projects. Please try again.");
            }
        }
        
        private async Task UpdateProject()
        {
            _console.WriteLine("==== Update Project ====");

            _console.Write("Enter project ID to update: ");
            var projectId = _console.ReadLine();

            _console.Write("Enter new title: ");
            var title = _console.ReadLine();

            _console.Write("Enter new name: ");
            var name = _console.ReadLine();

            _console.Write("Enter new description: ");
            var description = _console.ReadLine();

            var updatedProject = new Project
            {
                Title = title,
                Name = name,
                Description = description
            };

            bool success = await _projectService.UpdateProjectAsync(jwtToken, projectId, updatedProject);

            if (success)
            {
                _console.WriteLine("Project updated successfully!");
            }
            else
            {
                _console.WriteLine("Failed to update the project. Please try again.");
            }
        }
        
        private async Task CreateTeam()
        {
            _console.WriteLine("==== Create Team ====");

            _console.Write("Enter team name: ");
            var name = _console.ReadLine();

            var team = new Team
            {
                Data = new TeamData { Name = name, Admins = new List<string>(), Members = new List<string>() },
                Metadata = new TeamMetadata { SsoTeam = false }
            };

            bool success = await _teamService.CreateTeamAsync(jwtToken, team);

            if (success)
            {
                _console.WriteLine("Team created successfully!");
            }
            else
            {
                _console.WriteLine("Failed to create the team. Please try again.");
            }
        }

        private void QuitApplication()
        {
        }
    }
}
