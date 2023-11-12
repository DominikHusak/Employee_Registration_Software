Databáze pro vedení skladu

Použité technologie: C# MSSQL

Téma projektu: Program pracuje s databází, ve které jsou evidováni
zaměstnanci a také směny. Celá databáze rozlišuje tři role které může
uživatel nabývat a to \"dbSpravce\", který je pouze jeden, dále
\"Vedouci\", kteří mají přehled nad zaměstnanci pracujícími na skladě
\"guest\". Každý z uživatelů má dané funkce, které může na základě své
role provádět.

Funkce: Login - Při spuštění programu se každý uživatel přihlašuje
pomocí svého jména a hesla. Poté co je přihlášen se mu na základě jeho
role zobrazují funkce pro provedení.

dbSpravce(UserAccountManager) - Spravce může provádět základní CRUD
funkce jako create, delete, update a read. Může Přidávat nové uživatele
a také je mazat nebo upravovat jejich učty. Také může zobrazovat všechny
uživatelské učty a směny které jsou v databázi.

Vedouci(SuperVisorManager) - Vedoucí si může zobrazovat svůj profil aby
měl přehled nad svými údaji a změnit heslo svého účtu. Může také provést
update na uživatelích s rolí \"guest\", kdy může změnit plat, oddělení a
roli(funkce pro povýšení zaměstnance). Dále také může zaměstnancům
přidávt i odebírat směny. Pro přehled všech směn může podle jména
zaměstnance získat přehled směn.

Zaměstnanec/Skladnik(GuestAccountManager) - Zaměstnanec může si také
může zobrazovat svůj profil a měnit své heslo. Také si může zobrazovat
přehled směn.

Instalace a použití: Po instalaci a otevření projektu je důležité mít v connectionString nastavené sprácné údaje k databázi (Předem nastaveno).
Pokud je vše připraveno stačí jen program spustit a řídit se pokynama v konzoli. 
