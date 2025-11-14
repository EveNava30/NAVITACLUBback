using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using ReservasNC.Application.Interfaces;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Domain.Entities;
using ReservasNC.Domain.Interfaces.Repositories;
using System.Linq;

namespace ReservasNC.Infrastructure.Notifications
{
    public class FcmNotificationService : INotificationService
    {
        private readonly IFcmTokenRepository _repo;

        public FcmNotificationService(IFcmTokenRepository repo)
        {
            _repo = repo;

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("firebase-key.json")
                });
            }
        }

        public async Task RegisterFcmTokenAsync(int idUsuario, string token)
        {
            await _repo.SaveTokenAsync(new FcmToken
            {
                IdUsuario = idUsuario,
                Token = token
            });
        }

        public async Task SendPushToUserAsync(int idUsuario, string titulo, string mensaje)
        {
            var tokenEntity = await _repo.GetTokenByUserAsync(idUsuario);
            if (tokenEntity is null) return;

            var message = new Message
            {
                Token = tokenEntity,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = mensaje
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
