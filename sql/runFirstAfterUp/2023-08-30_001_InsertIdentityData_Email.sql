if not exists (select 1 from IdentityResources where name = 'email')
       INSERT INTO [dbo].[IdentityResources](
              [Enabled]
              ,[Name]
              ,[DisplayName]
              ,[Description]
              ,[Required]
              ,[Emphasize]
              ,[ShowInDiscoveryDocument]
              ,[Created]
              ,[Updated]
              ,[NonEditable])
       VALUES
       (
              1, --[Enabled]
              'email', --[Name]
              'Your email', --[DisplayName]
              'Your email', --[Description]
              1, --[Required]
              1, --[Emphasize]
              1, --[ShowInDiscoveryDocument]
              getutcdate(), --[Created]
              getutcdate(), --[Updated]
              1 --[NonEditable]
       )
