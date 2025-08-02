using TypeSpec.OpenAPI;

namespace OpenAI;

union ModerationAppliedInputType {
  string,
  text: "text",
  image: "image",
}

model CreateModerationResponseResult {
  /** Whether any of the below categories are flagged. */
  flagged: boolean;

  /** A list of the categories, and whether they are flagged or not. */
  categories: {
    /** Content that expresses, incites, or promotes hate based on race, gender, ethnicity, religion, nationality, sexual orientation, disability status, or caste. Hateful content aimed at non-protected groups (e.g., chess players) is harassment. */
    hate: boolean;

    /** Hateful content that also includes violence or serious harm towards the targeted group based on race, gender, ethnicity, religion, nationality, sexual orientation, disability status, or caste. */
    `hate/threatening`: boolean;

    /** Content that expresses, incites, or promotes harassing language towards any target. */
    harassment: boolean;

    /** Harassment content that also includes violence or serious harm towards any target. */
    `harassment/threatening`: boolean;

    /** Content that includes instructions or advice that facilitate the planning or execution of wrongdoing, or that gives advice or instruction on how to commit illicit acts. For example, "how to shoplift" would fit this category. */
    illicit: boolean;

    /** Content that includes instructions or advice that facilitate the planning or execution of wrongdoing that also includes violence, or that gives advice or instruction on the procurement of any weapon. */
    `illicit/violent`: boolean;

    /** Content that promotes, encourages, or depicts acts of self-harm, such as suicide, cutting, and eating disorders. */
    `self-harm`: boolean;

    /** Content where the speaker expresses that they are engaging or intend to engage in acts of self-harm, such as suicide, cutting, and eating disorders. */
    `self-harm/intent`: boolean;

    /** Content that encourages performing acts of self-harm, such as suicide, cutting, and eating disorders, or that gives instructions or advice on how to commit such acts. */
    `self-harm/instructions`: boolean;

    /** Content meant to arouse sexual excitement, such as the description of sexual activity, or that promotes sexual services (excluding sex education and wellness). */
    sexual: boolean;

    /** Sexual content that includes an individual who is under 18 years old. */
    `sexual/minors`: boolean;

    /** Content that depicts death, violence, or physical injury. */
    violence: boolean;

    /** Content that depicts death, violence, or physical injury in graphic detail. */
    `violence/graphic`: boolean;
  };

  /** A list of the categories along with their scores as predicted by model. */
  category_scores: {
    /** The score for the category 'hate'. */
    hate: float32;

    /** The score for the category 'hate/threatening'. */
    `hate/threatening`: float32;

    /** The score for the category 'harassment'. */
    harassment: float32;

    /** The score for the category 'harassment/threatening'. */
    `harassment/threatening`: float32;

    /** The score for the category 'illicit'. */
    illicit: float32;

    /** The score for the category 'illicit/violent'. */
    `illicit/violent`: float32;

    /** The score for the category 'self-harm'. */
    `self-harm`: float32;

    /** The score for the category 'self-harm/intent'. */
    `self-harm/intent`: float32;

    /** The score for the category 'self-harm/instructions'. */
    `self-harm/instructions`: float32;

    /** The score for the category 'sexual'. */
    sexual: float32;

    /** The score for the category 'sexual/minors'. */
    `sexual/minors`: float32;

    /** The score for the category 'violence'. */
    violence: float32;

    /** The score for the category 'violence/graphic'. */
    `violence/graphic`: float32;
  };

  /** A list of the categories along with the input type(s) that the score applies to. */
  category_applied_input_types: {
    /** The applied input type(s) for the category 'hate'. */
    hate: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'hate/threatening'. */
    `hate/threatening`: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'harassment'. */
    harassment: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'harassment/threatening'. */
    `harassment/threatening`: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'illicit'. */
    illicit: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'illicit/violent'. */
    `illicit/violent`: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'self-harm'. */
    `self-harm`: (ModerationAppliedInputType.text | ModerationAppliedInputType.image)[];

    /** The applied input type(s) for the category 'self-harm/intent'. */
    `self-harm/intent`: (ModerationAppliedInputType.text | ModerationAppliedInputType.image)[];

    /** The applied input type(s) for the category 'self-harm/instructions'. */
    `self-harm/instructions`: (ModerationAppliedInputType.text | ModerationAppliedInputType.image)[];

    /** The applied input type(s) for the category 'sexual'. */
    sexual: (ModerationAppliedInputType.text | ModerationAppliedInputType.image)[];

    /** The applied input type(s) for the category 'sexual/minors'. */
    `sexual/minors`: ModerationAppliedInputType.text[];

    /** The applied input type(s) for the category 'violence'. */
    violence: (ModerationAppliedInputType.text | ModerationAppliedInputType.image)[];

    /** The applied input type(s) for the category 'violence/graphic'. */
    `violence/graphic`: (ModerationAppliedInputType.text | ModerationAppliedInputType.image)[];
  };
}
