// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

function DownloadFileFromUrl(fileUrl) {
    var link = document.createElement('a');
    link.href = fileUrl;
    link.download = '';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function GetVisibleRowCount() {
    const container = document.getElementById("body-content");
    if (!container) 
        return 10; // Rückfallwert, solange der Inhaltsbereich noch nicht gerendert ist

    const rect = container.getBoundingClientRect();
    const availableHeight = window.innerHeight - rect.top - 340;
    return Math.floor(availableHeight / 36);
};
