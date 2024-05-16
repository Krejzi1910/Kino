using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Bilet
{
    public string Film { get; set; }
    public string GodzinaFilmu { get; set; }
    public decimal Cena { get; set; }
    public int Ilość { get; set; }
}

class Zamówienie
{
    public Dictionary<Bilet, int> ListaBiletów { get; set; }
    public string SposóbPłatności { get; set; }
    public decimal KwotaCałkowita { get; set; }

    public void ObliczKwotęBiletów()
    {
        KwotaCałkowita = 0;
        foreach (var kvp in ListaBiletów)
        {
            KwotaCałkowita += kvp.Key.Cena * kvp.Value;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Witaj w kinie!");
        Console.WriteLine("Dostępne filmy:");

        List<Bilet> bilety = WczytajBiletyZPliku(@"C:\Users\DELL\source\repos\PracaDomowa1\PracaDomowa1\repertuar.json");
        if (bilety == null)
        {
            Console.WriteLine("Nie udało się wczytać filmów. Kino jest chwilowo niedostępne.");
            return;
        }

        foreach (var bilet in bilety)
        {
            Console.WriteLine($"{bilet.Film} - {bilet.Cena} ZŁ");
        }

        Zamówienie zamówienie = UtwórzNoweZamówienie(bilety);
        if (zamówienie == null)
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

    static List<Bilet> WczytajBiletyZPliku(string nazwaPliku)
    {
        try
        {
            string jsonData = File.ReadAllText(nazwaPliku);
            return JsonConvert.DeserializeObject<List<Bilet>>(jsonData);
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

        while (true)
        {
            Console.WriteLine("Wybierz film na który chcesz kupić bilet (a następnie 4 aby przejść do płatności):");
            for (int i = 0; i < bilety.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {bilety[i].Film} - {bilety[i].Cena} ZŁ");
            }
            Console.Write("Twój wybór: ");
            string wybór = Console.ReadLine();
            if (wybór == "4")
            {
                break;
            }
            else if (int.TryParse(wybór, out int numerBiletu) && numerBiletu > 0 && numerBiletu <= bilety.Count)
            {
                Console.Write("Podaj ilość: ");
                if (int.TryParse(Console.ReadLine(), out int ilość) && ilość > 0)
                {
                    Bilet wybranyBilet = bilety[numerBiletu - 1];
                    zamówienie.ListaBiletów.Add(wybranyBilet, ilość);
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

        while (true)
        {
            Console.Write("Wybierz swój sposób płatności (wpisz 'gotówka' albo 'karta'): ");
            string sposóbPłatności = Console.ReadLine().ToLower();
            if (sposóbPłatności == "gotówka" || sposóbPłatności == "karta")
            {
                zamówienie.SposóbPłatności = sposóbPłatności;
                break;
            }
            else
            {
                Console.WriteLine("Nieprawidłowy sposób płatności!. Wybierz 'gotówka' albo 'karta'.");
            }
        }

        zamówienie.ObliczKwotęBiletów();

        return zamówienie;
    }

    static void WyświetlSzczegółyZamówienia(Zamówienie zamówienie)
    {
        Console.WriteLine("Bilety:");
        foreach (var kvp in zamówienie.ListaBiletów)
        {
            Console.WriteLine($"{kvp.Key.Film} - {kvp.Value} szt. - {kvp.Key.Cena * kvp.Value} ZŁ");
        }
        Console.WriteLine($"Sposób płatności: {zamówienie.SposóbPłatności}");
        Console.WriteLine($"Całkowita kwota do zapłaty: {zamówienie.KwotaCałkowita} ZŁ");
    }

    static bool ZapiszZamówienieDoPliku(Zamówienie zamówienie, string nazwaPliku)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(zamówienie, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(nazwaPliku, jsonData);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas zapisywania biletów: {ex.Message}");
            return false;
        }
    }
}