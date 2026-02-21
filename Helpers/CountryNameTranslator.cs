namespace CountryRiskAI.API.Helpers;

public static class CountryNameTranslator
{
    private static readonly Dictionary<string, string> SpanishToEnglish = new()
    {
        { "Colombia", "Colombia" },
        { "México", "Mexico" },
        { "Méjico", "Mexico" },
        { "Estados Unidos", "United States" },
        { "Reino Unido", "United Kingdom" },
        { "España", "Spain" },
        { "Francia", "France" },
        { "Alemania", "Germany" },
        { "Italia", "Italy" },
        { "Brasil", "Brazil" },
        { "Argentina", "Argentina" },
        { "Chile", "Chile" },
        { "Perú", "Peru" },
        { "Venezuela", "Venezuela" },
        { "Ecuador", "Ecuador" },
        { "Bolivia", "Bolivia" },
        { "Paraguay", "Paraguay" },
        { "Uruguay", "Uruguay" },
        { "Costa Rica", "Costa Rica" },
        { "Panamá", "Panama" },
        { "Guatemala", "Guatemala" },
        { "Honduras", "Honduras" },
        { "El Salvador", "El Salvador" },
        { "Nicaragua", "Nicaragua" },
        { "República Dominicana", "Dominican Republic" },
        { "Cuba", "Cuba" },
        { "Jamaica", "Jamaica" },
        { "Puerto Rico", "Puerto Rico" },
        { "China", "China" },
        { "Japón", "Japan" },
        { "India", "India" },
        { "Rusia", "Russia" },
        { "Corea del Sur", "South Korea" },
        { "Corea del Norte", "North Korea" },
        { "Canadá", "Canada" },
        { "Australia", "Australia" },
        { "Nueva Zelanda", "New Zealand" },
        { "Sudáfrica", "South Africa" },
        { "Kenia", "Kenya" },
        { "Egipto", "Egypt" },
        { "Turquía", "Turkey" },
        { "Arabia Saudí", "Saudi Arabia" },
        { "Emiratos Árabes Unidos", "United Arab Emirates" },
        { "Israel", "Israel" },
        { "Irán", "Iran" },
        { "Irak", "Iraq" },
        { "Pakistán", "Pakistan" },
        { "Bangladesh", "Bangladesh" },
        { "Indonesia", "Indonesia" },
        { "Tailandia", "Thailand" },
        { "Vietnam", "Vietnam" },
        { "Filipinas", "Philippines" },
        { "Malasia", "Malaysia" },
        { "Singapur", "Singapore" },
        { "Suiza", "Switzerland" },
        { "Suecia", "Sweden" },
        { "Noruega", "Norway" },
        { "Dinamarca", "Denmark" },
        { "Finlandia", "Finland" },
        { "Polonia", "Poland" },
        { "Portugal", "Portugal" },
        { "Grecia", "Greece" },
        { "Países Bajos", "Netherlands" },
        { "Bélgica", "Belgium" },
        { "Austria", "Austria" },
        { "República Checa", "Czech Republic" },
        { "Rumania", "Romania" },
        { "Hungría", "Hungary" },
        { "Bulgaria", "Bulgaria" },
        { "Croacia", "Croatia" },
        { "Serbia", "Serbia" },
        { "Eslovaquia", "Slovakia" },
        { "Eslovenia", "Slovenia" },
        { "Lituania", "Lithuania" },
        { "Letonia", "Latvia" },
        { "Estonia", "Estonia" },
        { "Irlanda", "Ireland" },
        { "Islandia", "Iceland" },
        { "Luxemburgo", "Luxembourg" },
        { "Mónaco", "Monaco" },
        { "Liechtenstein", "Liechtenstein" },
        { "Malta", "Malta" },
        { "Chipre", "Cyprus" }
    };

    public static List<string> GetSearchVariants(string countryName)
    {
        var variants = new List<string>();
        var normalizedInput = countryName.Trim();
        var normalizedInputWithoutAccents = RemoveAccents(normalizedInput);
        
        // Buscar en el diccionario de forma case-insensitive y sin acentos
        KeyValuePair<string, string>? match = null;
        
        foreach (var kvp in SpanishToEnglish)
        {
            var keyWithoutAccents = RemoveAccents(kvp.Key);
            if (string.Equals(kvp.Key, normalizedInput, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kvp.Key, normalizedInputWithoutAccents, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(keyWithoutAccents, normalizedInput, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(keyWithoutAccents, normalizedInputWithoutAccents, StringComparison.OrdinalIgnoreCase))
            {
                match = kvp;
                break;
            }
        }

        // Si encontramos una traducción, priorizar el nombre en inglés
        if (match.HasValue)
        {
            var foundMatch = match.Value;
            if (!string.Equals(foundMatch.Key, foundMatch.Value, StringComparison.OrdinalIgnoreCase))
            {
                // Agregar primero la traducción al inglés (más probable que funcione)
                variants.Add(foundMatch.Value);
            }
            // Agregar el nombre original también
            if (!variants.Contains(normalizedInput, StringComparer.OrdinalIgnoreCase))
            {
                variants.Add(normalizedInput);
            }
        }
        else
        {
            // Si no hay traducción, empezar con el nombre original
            variants.Add(normalizedInput);
        }

        // También intentar con el nombre original sin acentos
        if (!variants.Contains(normalizedInputWithoutAccents, StringComparer.OrdinalIgnoreCase))
        {
            variants.Add(normalizedInputWithoutAccents);
        }

        return variants;
    }

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }
}
