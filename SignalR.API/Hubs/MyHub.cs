using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.Models;

namespace SignalR.API.Hubs
{
    public class MyHub : Hub
    {
        private readonly AppDbContext _context;

        public MyHub(AppDbContext context)
        {
            _context = context;
        }

        public static List<string> Messages { get; set; } = new List<string>();
        public static int ClientCount { get; set; } = 0;

        //Clients.All => Bütün abone olan clientlar için işlem yapılacağı zaman bu özellik kullanılır.
        //Clients.Caller => Sadece istek yapan client için yapılacağı zaman bu özellik kullanılır.
        //Clients.Group => Grup içerisinde bulunan clientlar kendiler içerisinde haberleşir.
        public async Task SendMessage(string message)
        {
            Messages.Add(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        //Mesajları okumak için (Static bir listeden olduğu gibi veri tabanından gelen bir listeyi de dönebiliriz.)
        public async Task GetMessages()
        {
            await Clients.All.SendAsync("ReceiveMessages", Messages);
        }

        //Hub a bağlanıldığında çalışır.
        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }

        //Hub'tan ayrıldığında çalışır.
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
            await base.OnDisconnectedAsync(exception);
        }


        //Groups Örnekleri

        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task SendNameByGroup(string Name, string teamName)
        {
            var team = _context.Teams.Where(x => x.Name == teamName).FirstOrDefault();
            if (team is not null)
                team.Users.Add(new User { Name = Name });
            else
            {
                var newTeam = new Team { Name = teamName };
                newTeam.Users.Add(new User { Name = Name });
                _context.Teams.Add(newTeam);
            }

            await _context.SaveChangesAsync();
            await Clients.Groups(teamName).SendAsync("ReceiveMessageByGroup", Name, teamName);
        }

        public async Task GetNamesByGroup()
        {
            var teams = _context.Teams.Include(x => x.Users).Select(x => new
            {
                teamName = x.Name,
                Users = x.Users.ToList()
            });

            await Clients.All.SendAsync("ReceiveNamesByGroup", teams);
        }
        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }


    }
}