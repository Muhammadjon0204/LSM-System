namespace Application.DTOs.DashboardDTOs;

public class RatingsDistributionDto
{
    public int OneStar { get; set; }
    public int TwoStars { get; set; }
    public int ThreeStars { get; set; }
    public int FourStars { get; set; }
    public int FiveStars { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}