using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NetworkNamingService
{
    private static List<string> _takenPlayerNames = new List<string>();
    
    private static List<string> _takenServerNames = new List<string>();

    public static string GetNewPlayerName()
    {
        List<string> availableNames = _playerNames.Except(_takenPlayerNames).ToList();
        if (availableNames.Count <= 0)
        {
            Debug.LogError("No more Player Names available!");
            return "";
        }

        int randomIndex = Random.Range(0, availableNames.Count);
        string chosenName = availableNames[randomIndex];
        _takenPlayerNames.Add(chosenName);
        return chosenName;
    }

    public static void FreePlayerName(string playerName)
    {
        if(_takenPlayerNames.Contains(playerName))
            _takenPlayerNames.Remove(playerName);
    }

    public static void FreeAllPlayerNames()
    {
        _takenPlayerNames.Clear();
    }
    
    public static string GetNewServerName()
    {
        List<string> availableNames = _serverNames.Except(_takenServerNames).ToList();
        if (availableNames.Count <= 0)
        {
            Debug.LogError("No more Server Names available!");
            return "";
        }

        int randomIndex = Random.Range(0, availableNames.Count);
        string chosenName = availableNames[randomIndex];
        _takenServerNames.Add(chosenName);
        return chosenName;
    }

    public static void ServerNameTaken(string serverName)
    {
        _takenServerNames.Add(serverName);
    }
    
    public static void FreeAllServerNames()
    {
        _takenServerNames.Clear();
    }

    //From https://www.famousscientists.org/popular/
    // and https://www.globalcitizen.org/en/content/17-top-female-scientists-who-have-changed-the-worl/
    private static List<string> _playerNames = new List<string>
    {
        "André-Marie Ampère",
        "Mary Anning",
        "Archimedes",
        "Francis Bacon",
        "Alexander Graham Bell",
        "Elizabeth Blackwell",
        "Niels Bohr",
        "Tycho Brahe",
        "Rachel Carson",
        "Jane Cooke Wright",
        "Nicolaus Copernicus",
        "Charles-Augustin de Coulomb",
        "Marie Curie",
        "John Dalton",
        "Charles Darwin",
        "René Descartes",
        "Jennifer Doudna",
        "Albert Einstein",
        "Gertrude Elion",
        "Euclid",
        "Leonhard Euler",
        "Michael Faraday",
        "Pierre de Fermat",
        "Fibonacci",
        "Benjamin Franklin",
        "Rosalind Franklin",
        "Katherine Freese",
        "Galileo Galilei",
        "Carl Friedrich Gauss",
        "Sophie Germain",
        "Jane Goodall",
        "Caroline Herschel",
        "Heinrich Hertz",
        "Hippocrates",
        "Grace Hopper",
        "Edwin Hubble",
        "James Hutton",
        "Hypatia",
        "Mae C. Jemison",
        "Irene Joliot-Curie",
        "Johannes Kepler",
        "Stephanie Kwolek",
        "Henrietta Leavitt",
        "Rita Levi-Montalcini",
        "Ada Lovelace",
        "James Clerk Maxwell",
        "Maria Goeppert Mayer",
        "Barbara McClintock",
        "Lise Meitner",
        "Dmitri Mendeleev",
        "Isaac Newton",
        "Florence Nightingale",
        "Alfred Nobel",
        "Max Planck",
        "Pythagoras",
        "Vera Rubin",
        "Ernest Rutherford",
        "Srinivasa Ramanujan",
        "Sara Seager",
        "Sau Lan Wu",
        "Linus Torvalds",
        "Alan Turing",
        "Alessandro Volta",
        "James Watt",
        "Chen-Ning Yang"
    };
    
    //https://www.topuniversities.com/university-rankings/university-subject-rankings/2020/natural-sciences
    private static List<string> _serverNames = new List<string>
    {
        "Massachusetts Institute of Technology",
        "Harvard University",
        "Stanford University",
        "University of Cambridge",
        "University of Oxford",
        "California Institute of Technology",
        "ETH Zurich",
        "University of California",
        "Imperial College London",
        "Princeton University",
        "University of Chicago",
        "The University of Tokyo",
        "National University of Singapore",
        "Tsinghua University",
        "Peking University",
        "University of Toronto",
        "Nanyang Technological University",
        "Yale University",
        "Lomonosov Moscow State University",
        "Kyoto University",
        "Columbia University",
        "Cornell University",
        "University of British Columbia",
        "Ecole Polytechnique",
        "Université PSL",
        "Technical University of Munich",
        "Seoul National University",
        "University of Illinois",
        "Sorbonne University",
        "Tokyo Institute of Technology",
        "University of Texas",
        "Ludwig-Maximilians-Universität München",
        "The Australian National University",
        "The University of Edinburgh",
        "Fudan University",
        "The Hong Kong University",
        "Northwestern University",
        "University of Michigan-Ann Arbor",
        "University of Washington",
        "The University of Melbourne",
        "Georgia Institute of Technology",
        "McGill University",
        "The University of Manchester",
        "The University of Sydney",
        "TU Graz"
    };
}
