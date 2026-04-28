# РосКомБанк — Android (in-memory)

Avalonia.Android 11.2.3 порт мобильного приложения РосКомБанк. Без БД: все данные
живут в `static List<...>` в ОЗУ и **сбрасываются при каждом перезапуске
процесса**, как и просили.

---

## Установка APK на телефон

APK подписан debug-ключом. Это значит:
- ставится на любой Android 6.0+ (API 23+)
- **нельзя** опубликовать в Play Market
- Android попросит разрешить установку «из неизвестных источников»

### Через `adb` (с компа)

```sh
adb install RosKomBank-android.apk
```

### Прямо на телефоне

1. Скиньте `RosKomBank-android.apk` на телефон (Telegram, Email, USB).
2. Откройте файл на телефоне.
3. Если просит — разрешите установку из этого источника
   (Настройки → Приложения → Спец. доступ → Установка неизвестных приложений).
4. Жмите «Установить».

После установки появится иконка **РосКомБанк**.

---

## Тестовый аккаунт

Создаётся автоматически при первом запуске:

| Поле | Значение |
|---|---|
| Телефон | `79001234567` |
| PIN | `1234` |

У него заранее есть: баланс `125 430,50 ₽`, действующий кредит `50 000 ₽`,
мат. капитал `586 946,72 ₽`, налоговый долг `1 240 ₽`, 3 операции в истории и
2 штрафа.

Можно и зарегистрировать новый аккаунт через экран «Нет аккаунта? Зарегистрироваться» —
он попадёт в тот же in-memory `Store` и будет доступен до перезапуска.

---

## Что внутри

9 разделов, как в десктопе:

- **Главная** — 4 карточки (баланс/кредит/мат.капитал/налоги) + быстрые действия + последние операции.
- **Счёт и карта** — банковская карта-визуал, реквизиты, пополнение счёта.
- **Перевод** — по номеру телефона + сумма + комментарий.
- **История** — операции, сгруппированные по месяцу.
- **Мат. капитал** — остаток, направления, заявка.
- **Кредиты** — активный кредит ИЛИ форма с калькулятором (12/24/36/60 мес).
- **Штрафы** — список с оплатой и формой добавления.
- **Налоги** — задолженность и оплата.
- **Профиль** — данные, смена PIN, выход.

Навигация — нижний таб-бар (5 главных) + боковое меню («Ещё») со всеми разделами.

---

## Особенности реализации

- **Без БД.** `Store` — статический класс с `List<User>`/`List<Transaction>`/`List<Fine>`.
  Все CRUD-операции — обычные `.Add(...)` / `.Remove(...)` к спискам.
- **При каждом перезапуске** Android может (а скорее всего и будет) убить процесс,
  и все изменения исчезнут. `SeedIfEmpty()` пересоздаст тестового пользователя.
- **Тема** Avalonia Fluent Dark, иконки `Material.Icons.Avalonia 2.1.10`.
- **Один Activity** (`MainActivity : AvaloniaMainActivity<App>`) и один `UserControl` —
  все экраны переключаются заменой `Content` корневого `ContentControl`.

---

## Сборка из исходников (если хочется свою копию APK)

Нужен **.NET 8 SDK** + workload `android` + Android SDK (cmdline-tools, platform-tools,
platforms/android-34, build-tools/34.0.0):

```sh
# .NET workload
dotnet workload install android

# Android SDK
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

Чтобы быстро ткнуть UI без эмулятора — есть отдельный desktop-проект:

```sh
dotnet run --project RosKomBank.Mobile.Dev/RosKomBank.Mobile.Dev.csproj
```

Откроется окно 420×760 с тем же UI.

---

## Структура

```
mobile/RosKomBank.Mobile/
├── Directory.Packages.props          # Centralized package versions
├── RosKomBank.Mobile/                # Shared Avalonia UI + models
│   ├── App.axaml(.cs)
│   ├── Models.cs                     # User, Transaction, Fine + static Store
│   ├── Helpers.cs                    # UI utilities (gradients, colors, icons)
│   └── Views/
│       ├── ShellView.cs              # Root navigation shell + snackbar
│       ├── LoginView.cs              # Phone + PIN login
│       ├── RegisterView.cs           # 6 fields + PIN confirmation
│       └── MainView.cs               # All 9 banking sections
├── RosKomBank.Mobile.Android/        # Android-specific entry point
│   ├── MainActivity.cs               # AvaloniaMainActivity<App>
│   └── Properties/AndroidManifest.xml
└── RosKomBank.Mobile.Dev/            # Desktop runner for quick UI iteration
```
