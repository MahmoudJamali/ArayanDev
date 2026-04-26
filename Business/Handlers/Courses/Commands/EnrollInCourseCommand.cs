using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;
using Entities.Concrete;
using DataAccess.Abstract;

namespace Business.Handlers.Courses.Commands
{
    public class EnrollInCourseCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
    }

    public class EnrollInCourseCommandHandler
        : IRequestHandler<EnrollInCourseCommand, bool>
    {
        private readonly ICourseEnrollmentRepository _repository;

        public EnrollInCourseCommandHandler(ICourseEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken)
        {
            var alreadyEnrolled = await _repository.ExistsAsync(request.UserId, request.CourseId);

            if (alreadyEnrolled)
                return false;

            var enrollment = new CourseEnrollment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CourseId = request.CourseId,
                EnrollDate = DateTime.UtcNow
            };

            await _repository.AddAsync(enrollment);

            return true;
        }
    }

    public class EnrollInCourseCommandValidator
        : AbstractValidator<EnrollInCourseCommand>
    {
        public EnrollInCourseCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.CourseId)
                .NotEmpty();
        }
    }
}



