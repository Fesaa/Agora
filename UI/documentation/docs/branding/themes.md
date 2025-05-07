# Themes

Agora is proud of its default theme, as it looks amazing! But we understand you may need to use your specific brand colours to display some of these pages to the wider public.

To make this possible, Agora has a theme system, which allows you to overwrite a bunch of css variables to style the webpages as you want.
The following variables can be overwritten.

You may also find the default theme in our [GitHub repo](https://github.com/Fesaa/Agora/blob/master/UI/Web/src/theme/themes/default.css).

Your css file should have the variables inside a `:root, :root .name` block.

### Colours
```css
    --background-color: #F5F3F5;

    --error-color: #BD362F;
    --warning-color: #FFECB5;

    --primary-color: #274690;
    --primary-color-dark-shade: #1F3A75;
    --primary-color-darker-shade: #182D5A;
    --primary-color-darkest-shade: #101F3F;

    --secundairy-color: #576CA8;
    --body-text-color: #302B27;
    --secondary-text-color: #F5F3F5;
    --primary-color-text: #F5F3F5;
    --text-color-secondary: #6c757d;

    --surface-card: #ffffff;
    --surface-hover: #f0f0f0;
    --surface-border: #dee2e6;

    --shadow-light: rgba(0, 0, 0, 0.05);
    --shadow-darker: rgba(0, 0, 0, 0.1);
```

### Text & Spacing
```css
    --title-font-size: 2rem;
    --subtitle-font-size: 1rem;
    --spacing: 0.25rem;
    font-family: "Spectral", serif;
```

### Disclaimer

We try to keep this page up to date but cannot guarantee this. The default css file in the GitHub repo is always the single source of truth.