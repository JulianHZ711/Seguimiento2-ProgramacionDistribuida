using Access.Application.Utilities.Pagination;
using FluentValidation.Results;

namespace Access.Tests.Application
{
    [TestClass]
    public sealed class PaginationRequestValidatorTests
    {
        private static readonly PaginationRequestValidator Validator = new();

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-15)]
        public void PageSize_ZeroOrNegative_IsInvalid(int pageSize)
        {
            ValidationResult result = Validator.Validate(new PaginationRequest(1, pageSize));

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(PaginationRequest.PageSize)));
        }

        [TestMethod]
        public void PageSize_AboveMaximum_IsCappedAndValid()
        {
            PaginationRequest request = new(1, 500);

            Assert.AreEqual(PaginationRequest.MAX_PAGE_SIZE, request.PageSize);
            Assert.IsTrue(Validator.Validate(request).IsValid);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-3)]
        public void PageNumber_ZeroOrNegative_MovesToFirstPage(int pageNumber)
        {
            PaginationRequest request = new(pageNumber, 10);

            Assert.AreEqual(1, request.PageNumber);
            Assert.IsTrue(Validator.Validate(request).IsValid);
        }

        [TestMethod]
        public void PageNumber_TooLarge_IsInvalid()
        {
            ValidationResult result = Validator.Validate(new PaginationRequest(int.MaxValue, PaginationRequest.MAX_PAGE_SIZE));

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(PaginationRequest.PageNumber)));
        }

        [TestMethod]
        public void Standart_IsValid()
        {
            Assert.IsTrue(Validator.Validate(PaginationRequest.Standart()).IsValid);
        }
    }
}
