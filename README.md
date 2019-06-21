# Project - Book Creator

## Type - Site for Book Writing

## Description

You are a reader and like to write. Book Creator is a platform
where you can share your work with the other people.
Users can Add or Delete Stories and Chapters in them,
Users can Follow and Unfollow other Stories,
Users receive Notification if the Story they follow get a new Chapter,
Users can Comment and Rate other Stories,
Users can Block and Unblock other Users,
Users can write Messages to other Users.
Moderators have all rights a Regular User has,
Moderators can moderate Stories Comments.
Administrators have all rights,
Administrators can do everything.

## Entities

### User
  - Id (string)
  - Nickname (string)
  - Stories (collection of Story)
  - Chapters (collection of Chapter)
  - Received Messages (collection of Message)
  - Sent Messages (collection of Message)
  - Followed Stories (collection of Story)
  - Comments (collection of Comment)
  - Notifications (collection of Notification)
  - Announcements (collection of Announcement)
  - Blocked Users (collection of User)
  - Blocked From (collection of User)
### Story
  - Id (string)
  - Title (string)
  - Image URL (string)
  - Summary (string)
  - Created On (DateTime)
  - Last Edited On (DateTime)
  - Chapters (collection of Chapter)
  - Followers (collection of User)
  - Comments (collection of Comment)
  - Ratings (collection of Book Rating)
  - Story Genre (Story Genre)
  - Author (User)
  - Rating (double)
  - Length (int)
### Chapter
  - Id (string)
  - Title (string)
  - Length (int)
  - Author (User)
  - Story (Story)
  - Content (string)
  - Created On (DateTime)
### Message
  - Id (string)
  - Sent On (DateTime)
  - Sender (User)
  - Receiver (User)
  - Text (string)
  - Is Readed (bool)
### Comment
  - Id (string)
  - User (User)
  - Story (Story)
  - Message (string)
  - Commented On (DateTime)
### Notification
  - Id (string)
  - Is Seen (bool)
  - Message (string)
  - User (User)
### Story Genre
  - Id (string)
  - Genre (string)
  - Stories (collection of Story)
### Announcement
  - Id (string)
  - Published On (DateTime)
  - Content (string)
  - Author (User)
### Book Rating
  - Rating (Story Rating)
  - Story (Story)
### Story Rating
  - Id (string)
  - Rating (double)
  - User (User)
  - Book Ratings (collection of BookRating)
