using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{

    public class CasilleroElectronicoRequest
    {
        public string No_Documento { get; set; }
        public string No_CasilleroElectronico { get; set; }
    }

    public class DatosCasilleroElectronico
    {
        public string No_Documento { get; set; }
        public string No_CasilleroElectronico { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Fecha_Nacimiento { get; set;}
        public string No_NIT { get; set; }
        public string Telefono_Celular { get; set; }
    }


    public class CasilleroElectronicoResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class CasilleroElectronicoResponse_data
    {
        public long userLockerId { get; set; }
        public Tcuser tcUser { get; set; }
        public Tclocker tcLocker { get; set; }
        public int statusId { get; set; }
        public DateTime? createdAt { get; set; }
        public long createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public long updatedBy { get; set; }
    }

    public class Tcuser
    {
        public long userId { get; set; }
        public string fullname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public object tcDocumentType { get; set; }
        public long documentNumber { get; set; }
        public string address { get; set; }
        public Tcmunicipality tcMunicipality { get; set; }
        public string birthday { get; set; }
        public string nit { get; set; }
        public Tcareacode tcAreaCode { get; set; }
        public int phone { get; set; }
        public Tcethnicgroup tcEthnicGroup { get; set; }
        public Tcmaritalstatus tcMaritalStatus { get; set; }
        public Tclanguage tcLanguage { get; set; }
        public Tcprofession tcProfession { get; set; }
        public Tcsex tcSex { get; set; }
        public int statusId { get; set; }
        public DateTime? createdAt { get; set; }
        public int createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public int updatedBy { get; set; }
        public object tcSubregion { get; set; }
        public object retypePassword { get; set; }
        public object tcRole { get; set; }
        public object token { get; set; }
        public object lockers { get; set; }
        public object menu { get; set; }
        public int random { get; set; }
    }

    public class Tcmunicipality
    {
        public int municipalityId { get; set; }
        public string municipalityDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
        public Tcdepartment tcDepartment { get; set; }
        public object createAt { get; set; }
    }

    public class Tcdepartment
    {
        public int departmentId { get; set; }
        public string departmentDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
        public Tccountry tcCountry { get; set; }
    }

    public class Tccountry
    {
        public int countryId { get; set; }
        public string countryDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
    }

    public class Tcareacode
    {
        public int areaCodeId { get; set; }
        public string areaCodeDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
    }

    public class Tcethnicgroup
    {
        public int ethnicGroupId { get; set; }
        public string ethnicGroupDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
    }

    public class Tcmaritalstatus
    {
        public int maritalStatusId { get; set; }
        public string maritalStatusDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
    }

    public class Tclanguage
    {
        public int languageId { get; set; }
        public string languageDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
    }

    public class Tcprofession
    {
        public int professionId { get; set; }
        public string professionDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
        public object createAt { get; set; }
    }

    public class Tcsex
    {
        public int sexId { get; set; }
        public string sexDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
        public object createAt { get; set; }
    }

    public class Tclocker
    {
        public int lockerId { get; set; }
        public object lockerDesc { get; set; }
        public long lockerNumber { get; set; }
        public object tcEntityType { get; set; }
        public Tclockertype tcLockerType { get; set; }
        public object nitEntity { get; set; }
        public object businessName { get; set; }
        public object tradename { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
        public object createAt { get; set; }
    }

    public class Tclockertype
    {
        public int lockerTypeId { get; set; }
        public string lockerTypeDesc { get; set; }
        public int statusId { get; set; }
        public int createdBy { get; set; }
        public object updatedAt { get; set; }
        public int updatedBy { get; set; }
        public object createAt { get; set; }
    }


}