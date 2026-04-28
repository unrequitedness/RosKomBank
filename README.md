# РосКомБанк

Это проект для сдачи сессии. Демо банковского приложения, можно посмотреть как
работают вход, регистрация, переводы, кредит, штрафы, налоги, мат. капитал.

В репе две версии:
- **десктоп** (Avalonia, .NET 8) - папка `src/`. С базой SQLite, данные сохраняются.
- **мобильный** (Avalonia.Android, APK) - папка `mobile/`. Без базы, всё в ОЗУ,
  при перезапуске сбрасывается.

Можно использовать как:
- учебный пример если кто то хочет посмотреть как делать UI на Avalonia (десктоп
  и мобила почти одинаково);
- стартовый шаблон для своего пет проекта (можно выкинуть лишние разделы и
  оставить только то что нужно);
- основу для курсовой / лабы по информатике если препод гоняет.

Тестовый аккаунт (создаётся сам при первом запуске):
- телефон `79001234567`
- PIN `1234`

Можно зарегать новый, всё работает.

## Как запустить десктоп

Нужен .NET 8 SDK.

```sh
git clone https://github.com/unrequitedness/RosKomBank.git
cd RosKomBank/src/RosKomBank
dotnet run -c Release
```

Под Arch Linux + GNOME ставим зависимости:

```sh
sudo pacman -S --needed dotnet-sdk dotnet-runtime aspnet-runtime sqlite \
                        noto-fonts noto-fonts-emoji ttf-dejavu fontconfig
```

Если хочется один бинарь без установленного .NET:

```sh
dotnet publish -c Release -r linux-x64 --self-contained true \
               -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
./bin/Release/net8.0/linux-x64/publish/RosKomBank
```

База лежит рядом с бинарём в файле `roskombank.db`. Удалить файл = сброс данных.

## Как запустить мобильную версию

Готовый APK можно собрать так (см. `mobile/README.md` для подробностей):

```sh
cd mobile/RosKomBank.Mobile
dotnet publish RosKomBank.Mobile.Android/RosKomBank.Mobile.Android.csproj \
    -c Release -f net8.0-android \
    -p:AndroidSdkDirectory=$ANDROID_HOME
```

Готовый файл будет в
`RosKomBank.Mobile.Android/bin/Release/net8.0-android/publish/ru.roskombank.mobile-Signed.apk`.

Установить на телефон через `adb install` или просто открыть APK на телефоне
(надо разрешить установку из неизвестных источников).

В мобильной версии нет SQLite, всё лежит в оперативке (`static List<>`), при
закрытии приложения данные пропадают и тестовый юзер пересоздаётся.

## Что внутри

Все основные банковские функции:
- вход и регистрация
- главный экран с балансом, кредитом, мат. капиталом и налогами
- счёт и карта (с пополнением)
- переводы по номеру телефона
- история операций
- мат. капитал с заявкой
- кредиты с калькулятором аннуитета (12, 24, 36, 60 мес)
- штрафы (можно добавлять и оплачивать)
- налоги (оплата задолженности)
- профиль со сменой PIN

## Стек

- .NET 8
- Avalonia 11.2 (UI, кроссплатформенный)
- Material.Icons.Avalonia 2.1 (иконки)
- EF Core 8 + SQLite (только в десктопе)
- Avalonia.Android (для APK)

## Структура

```
RosKomBank/
├── src/RosKomBank/                 # десктоп (.NET 8 + Avalonia + SQLite)
│   ├── Program.cs
│   ├── App.axaml(.cs)
│   ├── Models.cs                   # User / Transaction / Fine / BankContext
│   ├── Helpers.cs
│   ├── LoginWindow.cs
│   ├── RegisterWindow.cs
│   └── MainBankWindow.cs           # все 9 разделов
└── mobile/RosKomBank.Mobile/       # мобила (.NET 8 + Avalonia.Android, без БД)
    ├── RosKomBank.Mobile/          # общий код (модели + UI)
    ├── RosKomBank.Mobile.Android/  # APK entry point
    └── RosKomBank.Mobile.Dev/      # десктоп раннер для теста UI без эмулятора
```
