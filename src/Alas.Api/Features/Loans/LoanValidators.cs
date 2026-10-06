using FluentValidation;

namespace Alas.Api.Features.Loans;

public sealed class CreateLoanApplicationRequestValidator : AbstractValidator<CreateLoanApplicationRequest>
{
    public CreateLoanApplicationRequestValidator()
    {
        RuleFor(x => x.BranchCode)
            .NotEmpty().WithMessage("Branch code is required")
            .MaximumLength(20).WithMessage("Branch code must not exceed 20 characters");

        RuleFor(x => x.CisId)
            .NotEmpty().WithMessage("CIS ID is required")
            .MaximumLength(10).WithMessage("CIS ID must not exceed 10 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.LoanNo)
            .NotEmpty().WithMessage("Loan number is required")
            .MaximumLength(50).WithMessage("Loan number must not exceed 50 characters");

        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("Product code is required")
            .MaximumLength(20).WithMessage("Product code must not exceed 20 characters");

        RuleFor(x => x.Product)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(x => x.ProposedAmount)
            .GreaterThan(0).WithMessage("Proposed amount must be greater than 0");

        RuleFor(x => x.TermDays)
            .GreaterThan(0).WithMessage("Term days must be greater than 0");

        RuleFor(x => x.InterestRate)
            .GreaterThanOrEqualTo(0).WithMessage("Interest rate must be non-negative");

        RuleFor(x => x.LoanType)
            .NotEmpty().WithMessage("Loan type is required")
            .MaximumLength(20).WithMessage("Loan type must not exceed 20 characters");
    }
}

public sealed class UpdateLoanApplicationRequestValidator : AbstractValidator<UpdateLoanApplicationRequest>
{
    public UpdateLoanApplicationRequestValidator()
    {
        RuleFor(x => x.ProposedAmount)
            .GreaterThan(0).WithMessage("Proposed amount must be greater than 0");

        RuleFor(x => x.TermDays)
            .GreaterThan(0).WithMessage("Term days must be greater than 0");

        RuleFor(x => x.InterestRate)
            .GreaterThanOrEqualTo(0).WithMessage("Interest rate must be non-negative");

        RuleFor(x => x.Remarks)
            .MaximumLength(2000).WithMessage("Remarks must not exceed 2000 characters");

        RuleFor(x => x.AoRecommendation)
            .MaximumLength(2000).WithMessage("AO recommendation must not exceed 2000 characters");
    }
}

public sealed class SubmitLoanRequestValidator : AbstractValidator<SubmitLoanRequest>
{
    public SubmitLoanRequestValidator()
    {
        RuleFor(x => x.BranchCode)
            .NotEmpty().WithMessage("Branch code is required");

        RuleFor(x => x.CisId)
            .NotEmpty().WithMessage("CIS ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.LoanNo)
            .NotEmpty().WithMessage("Loan number is required");

        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("Product code is required");

        RuleFor(x => x.Product)
            .NotEmpty().WithMessage("Product name is required");

        RuleFor(x => x.ProposedAmount)
            .GreaterThan(0).WithMessage("Proposed amount must be greater than 0");

        RuleFor(x => x.TermDays)
            .GreaterThan(0).WithMessage("Term days must be greater than 0");

        RuleFor(x => x.InterestRate)
            .GreaterThanOrEqualTo(0).WithMessage("Interest rate must be non-negative");
    }
}