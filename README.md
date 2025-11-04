# ⚗️ Potion Panic

**Аркадная алхимическая головоломка на реакцию и память.**  
Игрок видит рецепт (последовательность ингредиентов) и должен быстро и точно добавить их в котёл.  
Ошибся или замедлился — котёл «закипает», растёт риск «взрыва».  
Серии безошибочных зелий дают бонусы и ускоряют прогресс.

---

## 🎮 Режимы игры

- **Endless** — бесконечный режим: создавай зелья, пока не ошибёшься.  
- **Challenge** — набор фиксированных уровней с нарастающей сложностью *(в разработке)*.  
- **Results (Leaderboard)** — таблица рекордов: самые быстрые и самые долгие серии.  
- **Language switcher (RU/ET)** — переключатель языка на главном экране.  

---

## 💾 Текущее хранилище

Пока используется **in-memory** (данные хранятся в памяти).  
Позже безболезненно подключается **SQLite** — уже реализовано хранилище `ResultsRepository`.

---

## 🧩 Структура проекта

```
/Models
  IngredientKind.cs
  RecipeStep.cs
  PotionRecipe.cs
  RunResult.cs
  GameConfig.cs
  LevelDefinition.cs

/Repositories
  LevelsRepository.cs

/Services
  GameSessionService.cs
  LocalizationService.cs
  MusicService.cs
  ResultsRepository.cs
  ServiceHelper.cs

/ViewModels
  MenuViewModel.cs
  GameViewModel.cs
  ResultViewModel.cs

/Views
  IntroPage.xaml
  MenuPage.xaml
  GamePage.xaml
  ResultsPage.xaml

/Resources
  /Strings
    AppResources.resx
    AppResources.ru.resx
    AppResources.et.resx
```

---

## 🧭 Навигация (Shell)

В проекте используется **Shell** для маршрутизации и навигации:  
стартовая страница — *intro/menu*, где боковое меню (`Flyout`) временно отключено.  
После нажатия **«Start»** Flyout включается навсегда — создаётся эффект аккуратного *first-run landing screen*.

📘 Документация:
- [Microsoft Learn — MAUI Shell](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/?view=net-maui-9.0)  
- [API Reference — Microsoft.Maui.Controls.Shell](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.shell?view=net-maui-9.0)

---

## 🎨 Палитра

| Название | HEX | Описание |
|-----------|------|-----------|
| **Primary Navy** | `#0B1630` | Очень тёмный синий |
| **Deep Space** | `#0E1D3F` | Глубокий космический синий |
| **Royal Blue** | `#122552` | Королевский оттенок синего |
| **Accent Gold** | `#D4AF37` / `#C9A227` | Акцент для текста и иконок |

---

## 🎬 Видео-интро (MP4)

Для анимации используется **MediaElement** из `CommunityToolkit.Maui`.  
Видео (`intro.mp4`, 1080×1920, 5–10 секунд) помещается в:

```
Platforms/Android/Resources/raw/intro.mp4
```

Преимущества:
- Плавная анимация на iOS и Android.  
- Минимум кода и зависимостей.

---

## 🧰 Установленные NuGet пакеты и плагины

### 1️⃣ **CommunityToolkit.Maui**

Позволяет использовать:
- `MediaElement` — для видео и анимации;
- дополнительные UI-компоненты (Alerts, Toasts, Converters и т.д.).

```csharp
builder
    .UseMauiCommunityToolkit()
    .UseMauiCommunityToolkitMediaElement();
```

📦 NuGet: [CommunityToolkit.Maui](https://www.nuget.org/packages/CommunityToolkit.Maui)

---

### 2️⃣ **sqlite-net-pcl**

Высокоуровневая обёртка (мини-ORM) над SQLite для .NET MAUI.  
Позволяет работать с базой как с объектами — без явных SQL-запросов.

```csharp
var conn = new SQLiteAsyncConnection(path);
await conn.CreateTableAsync<GameResult>();
await conn.InsertAsync(item);
```

📦 NuGet: [sqlite-net-pcl](https://www.nuget.org/packages/sqlite-net-pcl)

---

### 3️⃣ **SQLitePCLRaw.bundle_green**

«Батарейный» бандл для низкоуровневой SQLitePCLRaw.  
Подтягивает нативные бинарники и инициализацию SQLite.

- На iOS используется системный SQLite.  
- На Android/Windows/macOS — встроенный `e_sqlite3`.

📦 NuGet: [SQLitePCLRaw.bundle_green](https://www.nuget.org/packages/SQLitePCLRaw.bundle_green)

🔗 Обычно устанавливается **в паре** с `sqlite-net-pcl`.

---

### 4️⃣ **Plugin.Maui.Audio**

Даёт кроссплатформенное API для работы со звуками и музыкой.  
В проекте используется для фонового музыкального сопровождения.

```csharp
builder.Services.AddSingleton(AudioManager.Current);
builder.Services.AddSingleton<MusicService>();
```

📦 NuGet: [Plugin.Maui.Audio](https://www.nuget.org/packages/Plugin.Maui.Audio)

---

## 🗃️ Как это работает вместе

- `sqlite-net-pcl` — предоставляет API (методы, атрибуты, классы).  
- `SQLitePCLRaw.bundle_green` — добавляет сам движок SQLite.  
- Вместе они дают полноценное встроенное хранилище без настройки нативных библиотек.  

---

## 🔧 Сервисы проекта

| Сервис | Назначение |
|--------|-------------|
| **GameSessionService** | Управление именем игрока и временем начала сессии |
| **ResultsRepository** | Хранение и загрузка результатов игр (SQLite) |
| **LocalizationService** | Переключение языков RU/ET, сохранение выбора |
| **MusicService** | Воспроизведение фоновой музыки и звуков |
| **ServiceHelper** | Статический локатор сервисов для доступа через `ServiceHelper.Get<T>()` |

---

## 🧠 ViewModels

| ViewModel | Назначение |
|------------|-------------|
| **MenuViewModel** | Управление главным меню, выбор языка, начало игры |
| **GameViewModel** | Логика игрового процесса, счёт, рецепт, прогресс |
| **ResultViewModel** | Отображение таблицы результатов и обновление данных |

---

## 🪄 Особенности

- Полная **локализация** через `AppResources.resx` (русский и эстонский).  
- **Анимации и эффекты** в игре (масштабирование котла, тряска при ошибках).  
- **Хранение настроек** с помощью `Preferences` (имя игрока, язык).  
- Поддержка **видеофона и фоновой музыки**.  
- **SQLite-поддержка** и архитектура, готовая к расширению (Leaderboard, Challenge и т.д.).  
- **ServiceHelper** обеспечивает лёгкий доступ к сервисам без лишнего кода.  

---

## 🚀 Запуск проекта

1. Открой решение **PotionPanic.sln** в Visual Studio 2022.  
2. Убедись, что установлены все NuGet-зависимости.  
3. Выбери целевую платформу (**Android**, **Windows**, **iOS**).  
4. Нажми ▶️ **Run / F5** — проект запустится.

---

## 🚧 В разработке / Планы

- 🌟 **Challenge Mode** — полноценные уровни с постепенным усложнением.  
- 💾 **Онлайн-лидерборд** — синхронизация рекордов между устройствами.  
- 🎶 **Звуковые эффекты и музыка** для различных действий (ошибка, успех, взрыв).  
- 🧙‍♀️ **Экран персонажа** — выбор аватара или алхимика.  
- 🧪 **Система бонусов и зелья-комбо** за серии без ошибок.  
- 🎨 **Доработка интерфейса** — анимированные кнопки, подсветка выбора.  
- ☁️ **Облачное хранилище** (Azure / Firebase) — для сохранения прогресса.  
- 🕹️ **Desktop-версия** — адаптация интерфейса под Windows.  

---

## 📄 Лицензия

Проект создан в учебных целях (Jekaterina Guzek).  
Свободно используется в рамках портфолио и образовательных проектов.

---

🧪 *Potion Panic* — это сочетание алхимии, реакции и памяти.  
Каждый рецепт — испытание внимательности, а каждая ошибка — шаг к совершенству. ✨
