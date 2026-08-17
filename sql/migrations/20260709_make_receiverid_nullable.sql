-- GitHub Copilot
-- Migration: ReceiverId in Messages nullable machen (empfohlen)
-- Führt dazu, dass globale Nachrichten mit NULL statt 0 gespeichert werden.

-- Prüfen/Anwenden:
-- 1) In der Datenbank ausführen (z. B. mysql client oder Admin-Tool) im korrekten Schema/DB.
-- 2) Falls ein Foreign Key auf ReceiverId existiert, ist normalerweise keine Änderung nötig.
--    Sollte die Änderung fehlschlagen, bitte FK-Namen ermitteln und ggf. temporär entfernen und danach wieder anlegen.

ALTER TABLE `Messages`
	MODIFY COLUMN `ReceiverId` INT NULL;

-- Optional: aktuelle Zeilen mit ReceiverId = 0 auf NULL setzen (nur falls 0 bisher als globaler Marker verwendet wurde)
-- UPDATE `Messages` SET `ReceiverId` = NULL WHERE `ReceiverId` = 0;
