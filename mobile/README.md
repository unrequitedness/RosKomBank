# РосКомБанк - Android (без БД)

Мобильная версия. Без базы, всё хранится в оперативке (`static List<User>`,
`List<Transaction>`, `List<Fine>`). Когда приложение закрывается, данные
сбрасываются и тестовый юзер пересоздаётся заново.

## Установка APK на телефон

APK подписан debug ключом. Это значит:
- ставится на любой Android 6.0 и выше (API 23+)
- в Play Market не выложить (но нам и не надо)
- Android попросит разрешить установку из неизвестных источников

### Через `adb` с компа

```sh
adb install RosKomBank-android.apk
```

### Прямо с телефона

1. Скиньте APK на телефон любым способом (Telegram, почта, USB).
2. Откройте файл.
3. Если просит, разрешите установку из этого источника
   (Настройки → Приложения → Спец. доступ → Установка неизвестных приложений).
4. Нажимаете «Установить».

После установки появится иконка **РосКомБанк**.

## Тестовый аккаунт

Создаётся сам при первом запуске:

| Поле | Значение |
|---|---|
| Телефон | `79001234567` |
| PIN | `1234` |

У него уже есть: баланс `125 430,50 ₽`, действующий кредит `50 000 ₽`,
мат. капитал `586 946,72 ₽`, налоговый долг `1 240 ₽`, 3 операции в истории и
2 штрафа.

Можно зарегать новый аккаунт через «Нет аккаунта? Зарегистрироваться», он
тоже попадёт в `Store` и будет работать пока приложение не закроют.

## Что есть

9 разделов как в десктопе:

- **Главная** - 4 карточки (баланс, кредит, мат. капитал, налоги) + быстрые
  действия + последние операции.
- **Счёт и карта** - визуал карты, реквизиты, пополнение.
- **Перевод** - по номеру телефона + сумма + комментарий.
- **История** - операции по месяцам.
- **Мат. капитал** - остаток, направления, заявка.
- **Кредиты** - активный кредит ИЛИ форма с калькулятором (12 / 24 / 36 / 60 мес).
- **Штрафы** - список с оплатой и форма добавления.
- **Налоги** - задолженность и оплата.
- **Профиль** - данные, смена PIN, выход.

Навигация:
- снизу таб бар (5 кнопок: Главная, Карта, Перевод, История, Ещё);
- сбоку выезжает меню со всеми 9 разделами.

## Что под капотом

- **Без БД.** Класс `Store` это статический контейнер с тремя `List<>`. Все
  операции это обычные `.Add()` / `.Remove()` к спискам. Никаких миграций, EF Core,
  SQLite нет.
- **Перезапуск = сброс.** Android может убить процесс в любой момент, и тогда
  `Store` обнулится. На старте `SeedIfEmpty()` пересоздаст тестового юзера.
- **Тема** Avalonia Fluent Dark, иконки `Material.Icons.Avalonia 2.1.10`.
- **Один Activity** (`MainActivity : AvaloniaMainActivity<App>`) и один
  `UserControl`. Все экраны переключаются заменой `Content` корневого
  `ContentControl`.

## Сборка из исходников

Нужен **.NET 8 SDK** + workload `android` + Android SDK (cmdline-tools,
platform-tools, platforms/android-34, build-tools/34.0.0).

```sh
# .NET workload
dotnet workload install android

# Android SDK (если ещё не стоит)
mkdir -p ~/android/cmdline-tools && cd ~/android
curl -O https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip
unzip commandlinetools-linux-*.zip -d cmdline-tools
mv cmdline-tools/cmdline-tools cmdline-tools/latest
export ANDROID_HOME=$HOME/android
yes | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --licenses
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager \
    "platform-tools" "platforms;android-34" "build-tools;34.0.0"

# Сборка APK
cd /path/to/RosKomBank/mobile/RosKomBank.Mobile
dotnet publish RosKomBank.Mobile.Android/RosKomBank.Mobile.Android.csproj \
    -c Release -f net8.0-android \
    -p:AndroidSdkDirectory=$ANDROID_HOME

# APK тут:
# RosKomBank.Mobile.Android/bin/Release/net8.0-android/publish/ru.roskombank.mobile-Signed.apk
```

Если хочется быстро потыкать UI без эмулятора, есть отдельный десктоп раннер:

```sh
dotnet run --project RosKomBank.Mobile.Dev/RosKomBank.Mobile.Dev.csproj
```

Откроется окно `420 x 760` с тем же интерфейсом.

## Структура

```
mobile/RosKomBank.Mobile/
├── Directory.Packages.props          # версии пакетов в одном месте
├── RosKomBank.Mobile/                # общий Avalonia UI и модели
│   ├── App.axaml(.cs)
│   ├── Models.cs                     # User, Transaction, Fine + static Store
│   ├── Helpers.cs                    # утилиты для UI (градиенты, цвета, иконки)
│   └── Views/
│       ├── ShellView.cs              # корневая навигация + снек бар
│       ├── LoginView.cs              # вход по телефону и PIN
│       ├── RegisterView.cs           # 6 полей + подтверждение PIN
│       └── MainView.cs               # все 9 банковских разделов
├── RosKomBank.Mobile.Android/        # точка входа Android
│   ├── MainActivity.cs               # AvaloniaMainActivity<App>
│   └── Properties/AndroidManifest.xml
└── RosKomBank.Mobile.Dev/            # десктоп раннер для теста UI
```
