SHOW PROCESSLIST;

SELECT CONCAT('KILL ', id, ';') AS kill_command 
FROM information_schema.processlist 
WHERE id != CONNECTION_ID();

