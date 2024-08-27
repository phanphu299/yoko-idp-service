UPDATE ClientSecrets
SET obfuscated_secret = CONCAT(
  SUBSTRING([value], 1, 3), -- get the first 3 characters of secret
  '****', -- add four asterisks
  SUBSTRING([value], LEN([value]) - 2, 3) -- get the last 3 characters of secret
);