-- by default client's claims will be prefixed by "client_", we update space to remove that prefix (null or empty not working)
update Clients set ClientClaimsPrefix = ' ' WHERE ClientClaimsPrefix IS NULL;