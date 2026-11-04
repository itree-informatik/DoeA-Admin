// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Data.Helper;

using System.Diagnostics;

/// <summary>Hilfsfunktionen der Fehlerbehandlung: Herkunft einer Exception und Auflösung übersetzter Meldungen mit Parametern.</summary>
public static class ExceptionHelper
{
    /// <summary>Klasse und Methode des ersten Stack-Frames ausserhalb von System.*, für Ressource/Methode in den Log-Einträgen.</summary>
    public static (string? ClassName, string? MethodName) GetExceptionOrigin(Exception ex)
    {
        var st = new StackTrace(ex, true);
        var frame = st.GetFrames().FirstOrDefault(f =>
            f.GetMethod()?.DeclaringType?.Namespace != null &&
            !f.GetMethod()?.DeclaringType?.Namespace?.StartsWith("System") == true);

        var method = frame?.GetMethod();
        var className = method?.DeclaringType?.FullName;
        var methodName = method?.Name;

        return (className, methodName);
    }

    /// <summary>Löst «KEY?param1?param2» in den übersetzten Text auf; die Parameter füllen die Platzhalter {0}, {1} …</summary>
    public static string GenerateExceptionMessage(string msg)
    {
        var newMessage = msg;

        if (msg.Contains("?"))
        {
            var splitParams = msg.Split('?');
            newMessage = TranslationHelper.Translate(splitParams[0]);

            if (newMessage.Contains("{0}"))
            {
                var parameters = splitParams.Skip(1).Select(param => (object)param).ToArray();
                newMessage = string.Format(newMessage, parameters);
            }
        }

        return newMessage;
    }
}
