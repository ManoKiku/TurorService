using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Review;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IReviewRepository reviewRepository,
        ITutorProfileRepository tutorProfileRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<ReviewService> logger)
    {
        _reviewRepository = reviewRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReviewDto> CreateReviewAsync(Guid userId, ReviewCreateRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Review text cannot be empty.");
        
        var tutorProfile = await _tutorProfileRepository.GetByIdAsync(request.TutorProfileId);
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found.");
        
        // Репетитор не может оставить отзыв сам себе
        if (tutorProfile.UserId == userId)
            throw new InvalidOperationException("Tutor cannot review themselves.");
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");
        
        // Проверка: пользователь уже оставлял отзыв этому репетитору?
        var existing = await _reviewRepository.GetByUserAndTutorAsync(userId, request.TutorProfileId);
        if (existing != null)
            throw new InvalidOperationException("You have already reviewed this tutor.");
        
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TutorProfileId = request.TutorProfileId,
            Rating = request.Rating,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangesAsync();
        
        var created = await _reviewRepository.GetByIdWithDetailsAsync(review.Id);
        var dto = _mapper.Map<ReviewDto>(created);
        dto.UserName = $"{user.FirstName} {user.LastName}";
        return dto;
    }
    
    public async Task<(IEnumerable<ReviewDto> Reviews, int TotalCount)> GetReviewsByTutorAsync(Guid tutorProfileId, int page, int pageSize)
    {
        var tutor = await _tutorProfileRepository.GetByIdAsync(tutorProfileId);
        if (tutor == null)
            throw new KeyNotFoundException("Tutor profile not found.");
        
        var reviews = await _reviewRepository.GetByTutorProfileIdAsync(tutorProfileId, page, pageSize);
        var total = await _reviewRepository.GetCountByTutorProfileIdAsync(tutorProfileId);
        
        var dtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

        foreach (var dto in dtos)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user != null)
                dto.UserName = $"{user.FirstName} {user.LastName}";
        }
        
        return (dtos, total);
    }
    
    public async Task<ReviewDto> UpdateReviewAsync(Guid reviewId, Guid userId, string userRole, ReviewUpdateRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Review text cannot be empty.");
        
        var review = await _reviewRepository.GetByIdWithDetailsAsync(reviewId);
        if (review == null)
            throw new KeyNotFoundException("Review not found.");
        
        if (userRole != "Admin" && review.UserId != userId)
            throw new UnauthorizedAccessException("You are not allowed to update this review.");
        
        review.Rating = request.Rating;
        review.Text = request.Text;
        review.UpdatedAt = DateTime.UtcNow;
        
        _reviewRepository.Update(review);
        await _reviewRepository.SaveChangesAsync();
        
        var updatedDto = _mapper.Map<ReviewDto>(review);
        var user = await _userRepository.GetByIdAsync(review.UserId);
        if (user != null)
            updatedDto.UserName = $"{user.FirstName} {user.LastName}";
        
        return updatedDto;
    }
    
    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, string userRole)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return false;
        
        if (userRole != "Admin" && review.UserId != userId)
            throw new UnauthorizedAccessException("You are not allowed to delete this review.");
        
        _reviewRepository.Remove(review);
        await _reviewRepository.SaveChangesAsync();
        return true;
    }
}