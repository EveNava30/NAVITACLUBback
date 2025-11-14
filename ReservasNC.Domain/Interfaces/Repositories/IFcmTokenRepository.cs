using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservasNC.Domain.Interfaces.Repositories
{
    public interface IFcmTokenRepository
    {
        Task SaveTokenAsync(FcmToken token);
        Task<string?> GetTokenByUserAsync(int idUsuario);
    }
}
