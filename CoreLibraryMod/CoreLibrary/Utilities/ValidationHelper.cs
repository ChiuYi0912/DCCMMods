
using System.Diagnostics;
using Serilog;
using Serilog.Core;

namespace CoreLibrary.Core.Utilities
{

    public static class ValidationHelper
    {

        public static T NotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException($"参数 '{paramName}' 为null");
            }
            return value;
        }


        public static string NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"参数 '{paramName}' 为null或空字符串", paramName);
            }
            return value;
        }


        public static string NotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"参数 '{paramName}' 为null或空白字符串", paramName);
            }
            return value;
        }


        public static string ValidLevelId(string levelId, string paramName = "levelId")
        {
            NotNullOrWhiteSpace(levelId, paramName);

            if (!GameConstants.IsKnownLevel(levelId))
            {
                throw new ArgumentException($"参数 '{paramName}' 不是有效的关卡ID: {levelId}", paramName);
            }

            return levelId;
        }


        public static string ValidBiomeId(string biomeId, string paramName = "biomeId")
        {
            NotNullOrWhiteSpace(biomeId, paramName);


            return biomeId;
        }


        public static T InRange<T>(T value, T min, T max, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"参数 '{paramName}' 必须在 {min} 到 {max} 之间");
            }
            return value;
        }


        public static T GreaterThan<T>(T value, T min, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(min) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"参数 '{paramName}' 必须大于 {min}");
            }
            return value;
        }


        public static T GreaterThanOrEqual<T>(T value, T min, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"参数 '{paramName}' 必须大于或等于 {min}");
            }
            return value;
        }


        public static T LessThan<T>(T value, T max, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(max) >= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"参数 '{paramName}' 必须小于 {max}");
            }
            return value;
        }


        public static T LessThanOrEqual<T>(T value, T max, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"参数 '{paramName}' 必须小于或等于 {max}");
            }
            return value;
        }

        public static ICollection<T> NotEmpty<T>(ICollection<T> collection, string paramName)
        {
            NotNull(collection, paramName);

            if (collection.Count == 0)
            {
                throw new ArgumentException($"集合 '{paramName}' 不能为空", paramName);
            }

            return collection;
        }


        public static ICollection<T> NoNullElements<T>(ICollection<T> collection, string paramName) where T : class
        {
            NotNull(collection, paramName);

            if (collection.Any(item => item is null))
            {
                throw new ArgumentException($"集合 '{paramName}' 不能包含null元素", paramName);
            }

            return collection;
        }

        public static T IsType<T>(object value, string paramName) where T : class
        {
            NotNull(value, paramName);

            if (value is not T typedValue)
            {
                throw new ArgumentException($"参数 '{paramName}' 必须是 {typeof(T).Name} 类型", paramName);
            }

            return typedValue;
        }


        public static string ValidEntityId(string entityId, string paramName = "entityId")
        {
            NotNullOrWhiteSpace(entityId, paramName);

            if (!System.Text.RegularExpressions.Regex.IsMatch(entityId, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException($"参数 '{paramName}' 必须是有效的实体ID（只允许字母、数字和下划线）", paramName);
            }

            return entityId;
        }


        public static string ValidWeaponId(string weaponId, string paramName = "weaponId")
        {
            return ValidEntityId(weaponId, paramName);
        }


        public static string ValidMonsterId(string monsterId, string paramName = "monsterId")
        {
            return ValidEntityId(monsterId, paramName);
        }


        public static T ValidConfigValue<T>(T value, string configName, Func<T, bool>? validator = null) where T : class
        {
            if (value is null)
            {
                throw new InvalidOperationException($"配置 '{configName}' 不能为null");
            }

            if (validator != null && !validator(value))
            {
                throw new InvalidOperationException($"配置 '{configName}' 的值无效");
            }

            return value;
        }


        public static dc.String NotNullHaxeString(dc.String value, string paramName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName, $"Haxe字符串参数 '{paramName}' 不能为null");
            }
            return value;
        }


        public static dc.pr.Level ValidLevel(dc.pr.Level level, string paramName = "level")
        {
            if (level is null)
            {
                throw new ArgumentNullException(paramName, $"关卡对象 '{paramName}' 不能为null");
            }

            if (level.destroyed)
            {
                throw new ArgumentException($"关卡对象 '{paramName}' 已被销毁", paramName);
            }

            return level;
        }


        public static dc.level.LevelMap ValidLevelMap(dc.level.LevelMap map, string paramName = "map")
        {
            if (map is null)
            {
                throw new ArgumentNullException(paramName, $"地图对象 '{paramName}' 不能为null");
            }

            return map;
        }


        public static dc.libs.Rand ValidRand(dc.libs.Rand rand, string paramName = "rand")
        {
            if (rand is null)
            {
                throw new ArgumentNullException(paramName, $"随机数生成器 '{paramName}' 不能为null");
            }

            return rand;
        }


        public static void That(bool condition, string message, string paramName = "")
        {
            if (!condition)
            {
                if (string.IsNullOrEmpty(paramName))
                {
                    throw new ArgumentException(message);
                }
                else
                {
                    throw new ArgumentException(message, paramName);
                }
            }
        }


        public static void State(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }


        public static T TryGetValue<T>(T? value, T defaultValue, string valueName, Action<string>? logger = null) where T : class
        {
            if (value is null)
            {
                var warning = $"{valueName} 为null，使用默认值";
                logger?.Invoke(warning);
                return defaultValue;
            }
            return value;
        }


        public static string TryGetString(string? value, string defaultValue, string valueName, Action<string>? logger = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                var warning = $"{valueName} 为null或空白，使用默认值: {defaultValue}";
                logger?.Invoke(warning);
                return defaultValue;
            }
            return value;
        }
    }
}