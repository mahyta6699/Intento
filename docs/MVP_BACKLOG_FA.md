# Intento — MVP Delivery Backlog

ساختار برنامه‌ریزی: Epic → Feature → User Story → Task. هر Task یک GitHub Issue مستقل است.

## قرارداد عنوان Issue
[E##-F##-US##-T##] عنوان تسک

شناسه‌ها به‌ترتیب Epic، Feature، User Story و Task را مشخص می‌کنند.

## اولویت‌ها
- P0: حیاتی برای مسیر اصلی و قابل استفاده شدن MVP.
- P1: ضروری برای تکمیل تجربه MVP پس از مسیر حیاتی.
- P2: مهم، اما قابل انجام پس از تثبیت هسته.
- P3: بهبود تکمیلی و غیرمسدودکننده انتشار اولیه.

اولویت در عنوان/بدنه Issue درج می‌شود. در حال حاضر labelهای اولویت به مخزن اضافه نشده‌اند؛ بنابراین اولویت به متن هر Issue متکی است.

## محدوده و فرض‌ها
- هدف فعلی Windows و استک C#، .NET 10، WinUI 3 است.
- منابع MVP: Application، URL و Folder با سازگاری Typeهای فعلی 0، 1 و 2.
- ذخیره‌سازی محلی JSON در Local AppData.
- Startup فقط خود Intento را باز می‌کند؛ اجرای خودکار Activity پیش‌فرض نیست.
- Cloud Sync، حساب کاربری، چندسکویی، اجرای موازی و گردش‌کار پیچیده خارج از MVP هستند.

## E01 — Core workspace data
### F01 — Activity configuration persistence
US01: به‌عنوان کاربر می‌خواهم گروه‌ها و آیتم‌ها بعد از راه‌اندازی مجدد باقی بمانند.
- T01: ذخیره امن JSON در Local AppData، بازیابی و مدیریت خرابی فایل (P0).
### F02 — Activity and item domain model
US01: به‌عنوان کاربر می‌خواهم Activity و آیتم‌های آن مدل و اعتبارسنجی مشخص داشته باشند.
- T01: تثبیت مدل‌ها، سازگاری Typeهای فعلی و validation (P0).

## E02 — Activity and workspace management
### F01 — Activity selection
US01: به‌عنوان کاربر می‌خواهم گروه‌های کاری را ببینم و انتخاب کنم.
- T01: UI انتخاب گروه، حالت خالی، loading و error (P0).
### F02 — Activity group editor
US01: به‌عنوان کاربر می‌خواهم گروه بسازم، ویرایش و حذف کنم.
- T01: CRUD گروه‌ها از UI (P1).
### F03 — Activity resource editor
US01: به‌عنوان کاربر می‌خواهم برنامه، لینک و پوشه را به گروه اضافه یا حذف کنم.
- T01: فرم آیتم، file/folder picker و اعتبارسنجی URL (P1).
### F04 — Item organization
US01: به‌عنوان کاربر می‌خواهم ترتیب و فعال‌بودن آیتم‌ها را مدیریت کنم.
- T01: reorder و enable/disable آیتم‌ها (P2).

## E03 — Workspace launching
### F01 — Resource launch dispatch
US01: به‌عنوان کاربر می‌خواهم با انتخاب Activity منابع آن باز شوند.
- T01: Launcher برای App، URL و Folder (P0).
### F02 — Launch result handling
US01: به‌عنوان کاربر می‌خواهم خطای یک آیتم مانع اجرای بقیه نشود.
- T01: نتیجه مستقل هر آیتم و ادامه اجرای گروه (P1).
### F03 — Launch feedback
US01: به‌عنوان کاربر می‌خواهم وضعیت و نتیجه اجرای گروه را ببینم.
- T01: نمایش busy/progress و خلاصه موفقیت/خطا (P1).

## E04 — Windows integration
### F01 — Startup registration
US01: به‌عنوان کاربر می‌خواهم در صورت تمایل Intento هنگام ورود به ویندوز اجرا شود.
- T01: تنظیم opt-in و ثبت Startup با رعایت رضایت ویندوز (P2).

## E05 — Quality and delivery
### F01 — Automated testing
US01: به‌عنوان نگهدارنده می‌خواهم منطق اصلی تست خودکار داشته باشد.
- T01: تست مدل، JSON و launch orchestration با xUnit و fake launcher (P1).
### F02 — Continuous integration
US01: به‌عنوان نگهدارنده می‌خواهم build و test هر تغییر خودکار بررسی شود.
- T01: GitHub Actions روی Windows runner (P2).
### F03 — Windows packaging
US01: به‌عنوان کاربر می‌خواهم نسخه قابل نصب Intento را دریافت کنم.
- T01: ساخت و اعتبارسنجی MSIX (P2).
### F04 — Release readiness
US01: به‌عنوان نگهدارنده می‌خواهم پیش از انتشار سناریوهای اصلی را بررسی کنم.
- T01: smoke-test و release checklist (P3).

## ترتیب اجرا
1. E01 و مسیر حیاتی E02-F01 و E03-F01.
2. E03-F02 و E02-F02/F03 برای کامل‌شدن تجربه اصلی.
3. E05-F01 هم‌زمان با توسعه سرویس‌ها؛ تست را به آخر موکول نکنید.
4. E03-F03 و E02-F04 برای بازخورد و سازمان‌دهی بهتر.
5. Startup و MSIX پس از تثبیت رفتار اصلی و با تست روی Windows واقعی.
6. اجرای release checklist روی محیط تمیز.

## Definition of Done
- Build موفق باشد.
- تست‌های مربوط اضافه/به‌روزرسانی و اجرا شده باشند.
- حالت‌های مرزی و خطا بررسی شوند.
- مستندات لازم به‌روز شوند.
- معیارهای پذیرش Issue پاس شده و PR/commit مرتبط شود.

## شمارش
5 Epic، 13 Feature، 13 User Story و 13 Task مستقل که هر کدام Issue جداگانه دارند.
