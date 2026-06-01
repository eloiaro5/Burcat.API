﻿using Burcat.API.Media;
using Burcat.API.System;
using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace Burcat.API
{
    public static partial class DataValidator
    {
        public static ValidationResult? ValidateMessagePost(MessagePost post)
        {
            if (post.ResponseTo is not null && post.ResponseTo.Value.Value == post.Identifier) return new("Cannot respond to the same message.");
            else if (post.Image is not null && post.Video is not null) return new("Cannot create a message with an image and a video attached.");
            else if (post.Image is BurcatIdentifier<Image> image && InterfaceOptions.Find(image).Politeness != post.Politeness) return new("Cannot create a post where the post and the image have distinct politenesses.");
            else if (post.Video is BurcatIdentifier<Video> video && InterfaceOptions.Find(video).Politeness != post.Politeness) return new("Cannot create a post where the post and the image have distinct politenesses.");
            else return ValidationResult.Success;
        }
        public static ValidationResult? ValidateImagePost(ImagePost post) => InterfaceOptions.UseProvider(provider => (from i in provider.Get<Image>() where i.Identifier == (Guid)post.Image select (Guid)i.Politeness).FirstOrDefault() == (Guid)post.Politeness ? new("The image and the post can't have distinct politenesses.") : ValidationResult.Success);
        public static ValidationResult? ValidatePostTags(string? tags)
        {
            if (tags is null) return ValidationResult.Success;
            else if (tags.Contains(' ')) return new("Tags cannot contain spaces.");
            else if (tags.Any(char.IsUpper)) return new("Tags must be in lower case.");
            else if (tags.Split(',').CountBy(t => t).Any(t => t.Value > 1)) return new("Tags cannot be repeated.");
            else return ValidationResult.Success;
        }

        public static ValidationResult? ValidateAdvertisementPost(AdvertisementPost post) => InterfaceOptions.UseProvider(provider => post.MaximumPoliteness is not null &&
            (from p in provider.Get<Politeness>() where p == post.MaximumPoliteness select p).First()
            <
            (from pt in provider.Get<IPost>() join p in provider.Get<Politeness>() on (Guid)pt.Politeness equals p.Identifier where pt.Identifier == post.Identifier select p).First()
            ? new("The maximum politeness needs to be null or, equal or over the post polinteness.") : ValidationResult.Success);
        
        public static ValidationResult? ValidatePostReactionReaction(string reaction)
        {
            if (string.IsNullOrWhiteSpace(reaction)) return new("Can only react to messages with a single emote.");

            reaction = reaction.Trim();
            if (StringInfo.ParseCombiningCharacters(reaction).Length != 1) return new("Can only react to messages with a single emote.");

            bool hasEmoji = false;
            foreach (Rune rune in reaction.EnumerateRunes())
            {
                if (IsEmojiRune(rune))
                {
                    hasEmoji = true;
                    continue;
                }

                if (!IsEmojiSequenceRune(rune)) return new("Can only react to messages with a single emote.");
            }

            return hasEmoji ? ValidationResult.Success : new("Can only react to messages with a single emote.");
        }

        private static bool IsEmojiRune(Rune rune)
        {
            int value = rune.Value;
            return value is >= 0x1F000 and <= 0x1FAFF
                or >= 0x2600 and <= 0x27BF
                or >= 0x2300 and <= 0x23FF
                or 0x00A9
                or 0x00AE
                or 0x2122
                or 0x2139
                or 0x3030
                or 0x303D
                or 0x3297
                or 0x3299;
        }

        private static bool IsEmojiSequenceRune(Rune rune)
        {
            int value = rune.Value;
            return value is 0x200D
                or 0x20E3
                or 0xFE0E
                or 0xFE0F
                or >= 0x1F3FB and <= 0x1F3FF
                or >= '0' and <= '9'
                or '#'
                or '*';
        }
    }
}
