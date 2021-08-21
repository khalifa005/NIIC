using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Comments.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }

        public int? ParentCommentId { get; set; }
        public CommentDto ParentComment { get; set; }
        public ICollection<CommentDto> Replies { get; set; }
    }
}
