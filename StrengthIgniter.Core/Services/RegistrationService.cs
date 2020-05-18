using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IRegistrationService
    {
    }

    public class RegistrationService : ServiceBase
    {
        #region CTOR
        private readonly IUserDataAccess _UserDal;
        private readonly IHashUtility _HashUtility;
        private readonly IEmailUtility _EmailUtility;
        private readonly ITemplateUtility _TemplateUtility;

        public RegistrationService(
            IUserDataAccess userDal,
            IHashUtility hashUtility,
            IEmailUtility emailUtility,
            ITemplateUtility templateUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILogUtility logger,
            Func<IDbConnection> fnGetConnection
        )
            : base(auditEventDal, logger, fnGetConnection)
        {
            _UserDal = userDal;
            _HashUtility = hashUtility;
            _EmailUtility = emailUtility;
            _TemplateUtility = templateUtility;
        }
        #endregion
    }
}
