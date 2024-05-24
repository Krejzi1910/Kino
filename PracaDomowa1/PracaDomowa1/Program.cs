using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class BiletBazowy
{
    public string Film { get; set; } 
    public string GodzinaFilmu { get; set; } 
    public decimal Cena { get; set; } 
}

// Klasa dziedzicząca z biletbazowy
class Bilet : BiletBazowy
{
    public int Ilość { get; set; } // Ilość biletów
}

class Zamówienie
{
    public Dictionary<Bilet, int> ListaBiletów { get; set; } // Lista Biletów w zamówieniu z podaną ilośćią
    public string SposóbPłatności { get; set; } 
    public decimal KwotaCałkowita { get; set; } 

    public void ObliczKwotęBiletów()
    {
        KwotaCałkowita = 0; // nadanie wartośći kwoty całkowitej
        foreach (var kvp in ListaBiletów) 
        {
            KwotaCałkowita += kvp.Key.Cena * kvp.Value; // Dodawanie ceny za ilość biletów do kwoty całkowitej
        }
    }
}

class Program
{
    static void Main(string[] args) // Metoda główna programu
    {
        Console.WriteLine("Witaj w kinie!"); 
        Console.WriteLine("Dostępne filmy:"); 

        // Wczytanie listy repertuaru z pliku json 
        List<Bilet> bilety = WczytajBiletyZPliku(@"C:\Users\DELL\source\repos\PracaDomowa1\PracaDomowa1\repertuar.json");
        if (bilety == null) // Sprawdzenie czy repertuar został poprawnie wczytany
        {
            Console.WriteLine("Nie udało się wczytać filmów. Kino jest chwilowo niedostępne."); 
            return; 
        }

        // Wyświetlenie dostępnych filmów
        foreach (var bilet in bilety)
        {
            Console.WriteLine($"{bilet.Film} - {bilet.Cena} ZŁ"); // Wyświetlenie tytułu filmu i ceny
        }

        // Utworzenie nowego zamówienia
        Zamówienie zamówienie = UtwórzNoweZamówienie(bilety);
        if (zamówienie == null) // sprawdza czy zamówienie zostało poprawnie utworzone
        {
            Console.WriteLine("Nie udało się zakupić biletów. Spróbuj ponownie później.");
            return; 
        }

        
        WyświetlSzczegółyZamówienia(zamówienie);

        if (ZapiszZamówienieDoPliku(zamówienie, "C:\\Users\\DELL\\source\\repos\\PracaDomowa1\\PracaDomowa1\\bilety.json"))
        {
            Console.WriteLine("Twoje bilety zostały pomyślnie zakupione i zapisane."); 
        }
        else
        {
            Console.WriteLine("Nie udało się zakupić biletów spróbuj ponownie pózniej."); 
        }
    }

    // wczytuje bilety z pliku json
    static List<Bilet> WczytajBiletyZPliku(string nazwaPliku)
    {
        try
        {
            string jsonData = File.ReadAllText(nazwaPliku); // Odczytuje zawartosć plików
            return JsonConvert.DeserializeObject<List<Bilet>>(jsonData); // Deserializacja json do listy biletów
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Wystąpił błąd podczas wczytywania filmów: {ex.Message}");
            return null;
        }
    }
    static Zamówienie UtwórzNoweZamówienie(List<Bilet> bilety)
    {
        Zamówienie zamówienie = new Zamówienie();
        zamówienie.ListaBiletów = new Dictionary<Bilet, int>(); 

        while (true) // Pętla do wyboru biletów
        {
            Console.WriteLine("Wybierz film na który chcesz kupić bilet (a następnie 4 aby przejść do płatności):");
            for (int i = 0; i < bilety.Count; i++) // Iteracja przez wszystkie dostępne bilety
            {
                Console.WriteLine($"{i + 1}. {bilety[i].Film} - {bilety[i].Cena} ZŁ"); // Wyświetlenie numeru tytułu i ceny biletu
            }
            Console.Write("Twój wybór: ");
            string wybór = Console.ReadLine(); // Odczytanie wyboru użytkownika
            if (wybór == "4") // Sprawdzenie czy użytkownik chce przejść do płatności
            {
                break;
            }
            else if (int.TryParse(wybór, out int numerBiletu) && numerBiletu > 0 && numerBiletu <= bilety.Count) // Sprawdzenie poprawności wyboru
            {
                Console.Write("Podaj ilość: ");
                if (int.TryParse(Console.ReadLine(), out int ilość) && ilość > 0) // Sprawdzenie poprawności ilości
                {
                    Bilet wybranyBilet = bilety[numerBiletu - 1]; // Pobranie wybranego biletu
                    zamówienie.ListaBiletów.Add(wybranyBilet, ilość); // Dodanie biletu do zamówienia
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa ilość. Spróbuj ponownie."); 
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
            }
        }

        while (true) // Pętla do wyboru sposobu płatności
        {
            Console.Write("Wybierz swój sposób płatności (wpisz 'gotówka' albo 'karta'): "); // Wyświetlenie prośby o wybór sposobu płatności
            string sposóbPłatności = Console.ReadLine().ToLower(); // użytkownik może pisac tylko małymi literami
            if (sposóbPłatności == "gotówka" || sposóbPłatności == "karta") // Sprawdzenie poprawności wyboru
            {
                zamówienie.SposóbPłatności = sposóbPłatności; // Ustawienie sposobu płatności w zamówieniu
                break;
            }
            else
            {
                Console.WriteLine("Nieprawidłowy sposób płatności!. Wybierz 'gotówka' albo 'karta'.");
            }
        }

        zamówienie.ObliczKwotęBiletów();

        return zamówienie; // zwraca utworzone zamówienie 
    }

    static void WyświetlSzczegółyZamówienia(Zamówienie zamówienie)
    {
        Console.WriteLine("Bilety:");
        foreach (var kvp in zamówienie.ListaBiletów) // iteracja wwszystkich biletów w zamówieniu 
        {
            Console.WriteLine($"{kvp.Key.Film} - {kvp.Value} szt. - {kvp.Key.Cena * kvp.Value} ZŁ"); // Wyświetlenie szczegółów każdego biletu
        }
        Console.WriteLine($"Sposób płatności: {zamówienie.SposóbPłatności}"); // Wyświetlenie sposobu płatności
        Console.WriteLine($"Całkowita kwota do zapłaty: {zamówienie.KwotaCałkowita} ZŁ"); // Wyświetlenie całkowitej kwoty do zapłaty
    }
    static bool ZapiszZamówienieDoPliku(Zamówienie zamówienie, string nazwaPliku)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(zamówienie, Newtonsoft.Json.Formatting.Indented); // Serializacja zamówienia do JSON
            File.WriteAllText(nazwaPliku, jsonData); // Zapisanie json do pliku
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas zapisywania biletów: {ex.Message}");
            return false;
        }
    }
}
