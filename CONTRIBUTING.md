# Introduction

First off, thank you for considering contributing to ContainrBot. It's people like you that make ContainrBot such a
great tool.

Following these guidelines helps to communicate that you respect the time of the developers managing and developing this
open source project. In return, they should reciprocate that respect in addressing your issue, assessing changes, and
helping you finalize your pull requests.

ContainrBot is an open source project, and we love to receive contributions from our community — you! There are many
ways to contribute, from writing tutorials or blog posts, improving the documentation, submitting bug reports and
feature requests or writing code which can be incorporated into ContainrBot itself.

# Ground Rules

### Responsibilities

* Before creating a pull request:
	* Ensure code builds successfully
	* Ensure unit tests pass
* Create issues for changes and enhancements that you wish to make. Discuss things transparently and get
	community feedback.
* Keep feature versions as small as possible, preferably one new feature per version.
* Be welcoming to newcomers and encourage diverse new contributors from all backgrounds. See
	the [Code of Conduct](https://github.com/josolanes/ContainrBot?tab=coc-ov-file#contributor-covenant-code-of-conduct).

# Your First Contribution

Unsure where to begin contributing to ContainrBot? You can start by looking through these beginner and help-wanted
issues:

* Beginner issues - issues which should only require a few lines of code, and a test or two.
* Help wanted issues - issues which should be a bit more involved than beginner issues.

Working on your first Pull Request? You can learn how from this quick
guide, [First Contributions](https://github.com/firstcontributions/first-contributions#first-contributions).

At this point, you're ready to make your changes! Feel free to ask for help; everyone is a beginner at first :smile_cat:

If a maintainer asks you to "rebase" your PR, they're saying that a lot of code has changed, and that you need to update
your branch so it's easier to merge.

# Getting started

1. Create your own fork of the code
2. Do the changes in your fork
3. If you like the change and think the project could use it:
	* Be sure you have followed the code style for the project.
	* Note
		the [Code of Conduct](https://github.com/josolanes/ContainrBot?tab=coc-ov-file#contributor-covenant-code-of-conduct).
	* Send a pull request
		* If your change adds or modifies features (such as a new chatbot, new orchestrator) please also update the
			readme to make it easier for other people to use

# How to report a bug

## Security Issues

If you find a security vulnerability, do NOT open an issue. Email joshuasolanes@gmail.com instead.

Any security issues should be submitted directly to joshuasolanes@gmail.com

In order to determine whether you are dealing with a security issue, ask yourself these two questions:

* Can I access something that's not mine, or something I shouldn't have access to?
* Can I disable something for other people?

If the answer to either of those two questions are "yes", then you're probably dealing with a security issue. Note that
even if you answer "no" to both questions, you may still be dealing with a security issue, so if you're unsure, just
email us at joshuasolanes@gmail.com.

## Issue Template

When filing an issue, make sure to answer these five questions:

1. What orchestrator are you using? (docker, kubernetes, etc.)
2. What operating system and processor architecture are you using?
3. What did you do?
4. What did you expect to see?
5. What did you see instead?

Before submitting an issue, please use the ContainrBotApi `/debug` endpoint and ensure everything looks as you'd expect.
This endpoint tries to be transparent about what the ContainrBot "knows" and if something looks incorrect, it may be a
source of your issue.

General questions can be submitted in [Discussions](https://github.com/josolanes/ContainrBot/discussions).

# How to suggest a feature or enhancement

The ContainrBot philosophy is to provide a means to easily and securely control containers from a chatbot.

ContainrBot is designed to be extensible and to support many orchestrators (docker, kubernetes, etc.) and chat
platforms (discord, slack, etc.) allowing flexibility by design.

We welcome new chatbot and orchestrator integrations, so please suggest them or even contribute!

If you find yourself wishing for a feature that doesn't exist in ContainrBot, you are probably not alone. There are
bound to be others out there with similar needs. Many of the features that ContainrBot has today have been added
because our users saw the need. Open an issue on our issues list on GitHub which describes the feature you would like to
see, why you need it, and how it should work.

# Code review process

The core team looks at Pull Requests on a regular basis, often within a day or two.

After feedback has been given we expect responses within two weeks. After two weeks we may close the pull request if it
isn't showing any activity.

# BONUS

## Code Style

Code style is enforced with an `.editorconfig` file that is verified on build. To auto-format the code per the
`.editorconfig` run `dotnet format` in the repo root

## Issue Labels

Please use issue labels when creating issues to help us categorize issues