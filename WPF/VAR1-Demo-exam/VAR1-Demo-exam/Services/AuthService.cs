using System;
using System.Collections.Generic;
using System.Linq;

namespace VAR1_Demo_exam.Services
{
    public enum AppRole
    {
        Unknown = 0,
        Director,
        Manager,
        Producer,
        Consumer,
        Carrier
    }

    public static class SessionInfo
    {
        public static Polzovatel CurrentUser { get; set; }
        public static List<AppRole> Roles { get; set; } = new List<AppRole>();
    }

    public class AuthService
    {
        private readonly DataServiceAdo<Polzovatel> _polzovatelService = new DataServiceAdo<Polzovatel>();
        private readonly DataServiceAdo<Rol_Polz> _rolPolzService = new DataServiceAdo<Rol_Polz>();
        private readonly DataServiceAdo<Rol> _rolService = new DataServiceAdo<Rol>();

        public bool Login(string idNumber, string password, out string errorMessage)
        {
            try
            {
                errorMessage = string.Empty;

                if (!long.TryParse(idNumber, out var userId))
                {
                    errorMessage = "IdNumber должен быть числом.";
                    return false;
                }

                var user = _polzovatelService.GetAll().FirstOrDefault(p => p.ID_Polz == userId);
                if (user == null)
                {
                    errorMessage = "Пользователь с таким IdNumber не найден.";
                    return false;
                }

                if (!string.Equals(user.Parol ?? string.Empty, password ?? string.Empty, StringComparison.Ordinal))
                {
                    errorMessage = "Неверный пароль.";
                    return false;
                }

                var roleIds = _rolPolzService.GetAll()
                    .Where(rp => rp.ID_Polz == user.ID_Polz)
                    .Select(rp => rp.ID_Rol)
                    .ToList();

                var roleMap = _rolService.GetAll()
                    .Where(r => roleIds.Contains(r.ID_Rol))
                    .Select(r => ParseRole(r.Rol1, r.ID_Rol))
                    .Where(r => r != AppRole.Unknown)
                    .Distinct()
                    .ToList();

                if (!roleMap.Any())
                {
                    roleMap.Add(AppRole.Manager);
                }

                SessionInfo.CurrentUser = user;
                SessionInfo.Roles = roleMap;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Ошибка авторизации: " + ex.Message;
                return false;
            }
        }

        private static AppRole ParseRole(string roleName, long roleId)
        {
            var value = (roleName ?? string.Empty).Trim().ToLowerInvariant();
            var normalized = value.Replace(" ", string.Empty).Replace("_", string.Empty).Replace("-", string.Empty);

            if (value.Contains("директор")) return AppRole.Director;
            if (value.Contains("менедж")) return AppRole.Manager;
            if (value.Contains("производ")) return AppRole.Producer;
            if (value.Contains("потреб")) return AppRole.Consumer;
            if (value.Contains("перевоз")) return AppRole.Carrier;

            if (normalized.Contains("direktor") || normalized == "director") return AppRole.Director;
            if (normalized.Contains("menedzher") || normalized.Contains("manager")) return AppRole.Manager;
            if (normalized.Contains("proizvoditel") || normalized.Contains("producer")) return AppRole.Producer;
            if (normalized.Contains("potrebitel") || normalized.Contains("consumer")) return AppRole.Consumer;
            if (normalized.Contains("perevozchik") || normalized.Contains("carrier")) return AppRole.Carrier;

            // Резервное сопоставление по ID роли (часто в учебных БД роли идут 1..5).
            if (roleId == 1) return AppRole.Director;
            if (roleId == 2) return AppRole.Manager;
            if (roleId == 3) return AppRole.Producer;
            if (roleId == 4) return AppRole.Consumer;
            if (roleId == 5) return AppRole.Carrier;

            return AppRole.Unknown;
        }
    }
}
