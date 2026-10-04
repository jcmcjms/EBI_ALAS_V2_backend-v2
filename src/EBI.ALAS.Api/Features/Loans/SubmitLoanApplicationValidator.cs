using FluentValidation;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class SubmitLoanApplicationValidator : AbstractValidator<SubmitLoanApplicationRequest>
{
    public SubmitLoanApplicationValidator()
    {
        RuleFor(x => x.Client).NotNull().SetValidator(new ClientSectionValidator());
        RuleFor(x => x.BranchType).NotNull();
        RuleFor(x => x.Loans).NotEmpty().ForEach(l => l.SetValidator(new LoanSectionValidator()));
        RuleFor(x => x.OutstandingLoans).NotNull();
    }
}

public sealed class ClientSectionValidator : AbstractValidator<ClientSection>
{
    public ClientSectionValidator()
    {
        RuleFor(x => x.CisId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
    }
}

public sealed class LoanSectionValidator : AbstractValidator<LoanSection>
{
    public LoanSectionValidator()
    {
        RuleFor(x => x.CreationTypeCode).NotNull();
        RuleFor(x => x.BranchCode).NotEmpty();
        RuleFor(x => x.LoanNo).NotEmpty();
        RuleFor(x => x.ProductCode).NotEmpty();
        RuleFor(x => x.Parameters).NotNull().SetValidator(new ParametersSectionValidator());
    }
}

public sealed class ParametersSectionValidator : AbstractValidator<ParametersSection>
{
    public ParametersSectionValidator()
    {
        RuleFor(x => x.Product).NotEmpty();
        RuleFor(x => x.ProposedAmount).GreaterThan(0);
        RuleFor(x => x.Term).GreaterThan(0);
        RuleFor(x => x.InterestRate).GreaterThan(0);
    }
}