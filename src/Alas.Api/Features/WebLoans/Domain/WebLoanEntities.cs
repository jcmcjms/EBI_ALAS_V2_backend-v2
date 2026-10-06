using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alas.Api.Features.WebLoans.Domain;

/// <summary>
/// Maps to existing webloan.dbo.cis_info table.
/// This is a read-only entity — the webloan database is owned by the WebLoan system.
/// </summary>
[Table("cis_info", Schema = "dbo")]
public sealed class CisInfo
{
    [Key]
    [Column("cis_no")]
    [MaxLength(10)]
    public string CisNo { get; set; } = string.Empty;

    [Column("first_name")]
    public string? FirstName { get; set; }

    [Column("middle_name")]
    public string? MiddleName { get; set; }

    [Column("last_name")]
    public string? LastName { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("appelation")]
    public string? Appelation { get; set; }

    [Column("birth_date")]
    public string? BirthDateRaw { get; set; }

    [Column("zip")]
    public string? Zip { get; set; }

    [Column("house_street")]
    public string? HouseStreet { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("state_province")]
    public string? StateProvince { get; set; }

    [Column("barangay")]
    public string? Barangay { get; set; }

    [Column("village")]
    public string? Village { get; set; }

    [Column("occupation")]
    public string? Occupation { get; set; }

    [Column("region_code")]
    public string? RegionCode { get; set; }

    [Column("division_code")]
    public string? DivisionCode { get; set; }

    [Column("station_code")]
    public string? StationCode { get; set; }

    [Column("employee_no")]
    public string? EmployeeNo { get; set; }
}

/// <summary>
/// Maps to existing webloan.dbo.cis_info_misc_data table.
/// </summary>
[Table("cis_info_misc_data", Schema = "dbo")]
public sealed class CisInfoMiscData
{
    public const string AgencyTypeIdCode = "AGENCY_TYPE";

    [Column("cis_no")]
    [MaxLength(10)]
    public string CisNo { get; set; } = string.Empty;

    [Column("id_code")]
    public string IdCode { get; set; } = string.Empty;

    [Column("value_str")]
    public string? ValueStr { get; set; }
}

/// <summary>
/// Maps to existing webloan.dbo.loan_acct_info table.
/// </summary>
[Table("loan_acct_info", Schema = "dbo")]
public sealed class LoanAcctInfo
{
    [Column("bk")]
    public string BankCode { get; set; } = string.Empty;

    [Column("bch")]
    public string BranchCode { get; set; } = string.Empty;

    [Column("acct_no")]
    public string AccountNo { get; set; } = string.Empty;

    [Column("cis_no")]
    public string CisNo { get; set; } = string.Empty;

    [Column("name")]
    public string? Name { get; set; }

    [Column("credit_limit")]
    public decimal? CreditLimit { get; set; }

    [Column("used_credit")]
    public decimal? UsedCredit { get; set; }

    [Column("borrower_type")]
    public string? BorrowerType { get; set; }

    [Column("mis_group2")]
    public string? MisGroup2 { get; set; }

    [Column("solicitor")]
    public string? Solicitor { get; set; }
}

/// <summary>
/// Maps to existing webloan.dbo.loan_product table.
/// </summary>
[Table("loan_product", Schema = "dbo")]
public sealed class LoanProductLookup
{
    [Key]
    [Column("id_code")]
    public string IdCode { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("expiration")]
    public DateTimeOffset? Expiration { get; set; }
}

/// <summary>
/// Maps to existing webloan.dbo.mis_group table.
/// </summary>
[Table("mis_group", Schema = "dbo")]
public sealed class MisGroup
{
    [Key]
    [Column("frp_id")]
    public int FrpId { get; set; }

    [Column("group_no")]
    public int GroupNo { get; set; }

    [Column("pid_code")]
    public string? PidCode { get; set; }

    [Column("id_code")]
    public string? IdCode { get; set; }

    [Column("grp_level")]
    public int? GrpLevel { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("description2")]
    public string? Description2 { get; set; }

    [Column("path")]
    public string? Path { get; set; }
}

/// <summary>
/// Maps to existing webloan.dbo.check_list_data table.
/// </summary>
[Table("check_list_data", Schema = "dbo")]
public sealed class CheckListData
{
    public const string LengthOfServiceItem = "CCR06";

    public static readonly string[] CocreeItems =
    [
        "CCR01", "CCR02", "CCR03", "CCR04", "CCR05", "CCR06", "CCR07"
    ];

    [Column("cis_no")]
    public string CisNo { get; set; } = string.Empty;

    [Column("check_list_item")]
    public string CheckListItem { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("submitted")]
    public DateTimeOffset? Submitted { get; set; }

    [Column("expiration")]
    public DateTimeOffset? Expiration { get; set; }
}

/// <summary>
/// Keyless entity for outstanding loan queries (raw SQL projection).
/// </summary>
public sealed class OutstandingLoanRow
{
    public string? LoanNo { get; set; }
    public decimal? Principal { get; set; }
    public decimal? PrincipalBalance { get; set; }
    public decimal? ComputedAmortAmount { get; set; }
    public DateTimeOffset? DateGranted { get; set; }
    public DateTimeOffset? DateMaturity { get; set; }
    public string? ProductCode { get; set; }
    public byte? StatusCode { get; set; }
    public string? ProductWithDescription { get; set; }
}

/// <summary>
/// Keyless entity for pending loan queries (raw SQL projection).
/// </summary>
public sealed class PendingLoanRow
{
    public string? LoanNo { get; set; }
    public decimal? Principal { get; set; }
    public decimal? GrantedRate { get; set; }
    public int? TotalTermDays { get; set; }
    public int? TotalAmortization { get; set; }
    public string? ProductWithDescription { get; set; }
    public string? LoanPurpose { get; set; }
    public byte? CreationType { get; set; }
    public string? CreationTypeLabel { get; set; }
    public decimal? CDocStamp { get; set; }
    public string? Nthp { get; set; }
    public DateTimeOffset? NthpDate { get; set; }
}