![alt text](https://37wz5x2r8vbh3om46wmfhy71-wpengine.netdna-ssl.com/wp-content/uploads/2016/07/Book-Creator-logo-landscape.png)
# Project - Book Creator

## Type - Site for Book Writing

## Description

You are a reader and like to write. Book Creator is a platform
where you can share your work with the other people.
Users can Add or Delete Books and Chapters in them,
Users can Follow and Unfollow other Books,
Users receive Notification if the Book they follow get a new Chapter,
Users can Comment and Rate other Books,
Users can Block and Unblock other Users,
Users can write Messages to other Users.
Moderators have all rights a Regular User has,
Moderators can moderate Books Comments.
Administrators have all rights,
Administrators can do everything.

## Entities

### User
  - Id (string)
  - Nickname (string)
  - Books (collection of Book)
  - Chapters (collection of Chapter)
  - Received Messages (collection of Message)
  - Sent Messages (collection of Message)
  - Followed Books (collection of Book)
  - Comments (collection of Comment)
  - Notifications (collection of Notification)
  - Announcements (collection of Announcement)
  - Blocked Users (collection of User)
  - Blocked From (collection of User)
### Book
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
  - Book Genre (Book Genre)
  - Author (User)
  - Rating (double)
  - Length (int)
### Chapter
  - Id (string)
  - Title (string)
  - Length (int)
  - Author (User)
  - Book (Book)
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
  - Book (Book)
  - Message (string)
  - Commented On (DateTime)
### Notification
  - Id (string)
  - Is Seen (bool)
  - Message (string)
  - User (User)
### Book Genre
  - Id (string)
  - Genre (string)
  - Books (collection of Book)
### Announcement
  - Id (string)
  - Published On (DateTime)
  - Content (string)
  - Author (User)
### Book Rating
  - Rating (Book Rating)
  - Book (Book)
### User Rating
  - Id (string)
  - Rating (double)
  - User (User)
  - Book Ratings (collection of BookRating)
