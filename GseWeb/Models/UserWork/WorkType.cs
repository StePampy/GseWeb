using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GseWeb.Models.UserWork
{
    public enum WorkType
    {
        Default,
        Lavoro,
        Straordinario,
        [Description("Non Giustificato")]
        NonGiustificato,
        Ferie,
        [Description("Permesso Retribuito")]
        PermessoRetribuito,
        [Description("Permesso Non Retribuito")]
        PermessoNonRetribuito,
        Malattia,
        Infortunio,
        Lutto,
        Recupero,
        Viaggio,
        [Description("Ore Senza Commesse")]
        NoCommesse,
        [Description("Non Approvate")]
        NoApproved,
    }
}