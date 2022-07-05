import puppeteer from 'puppeteer';
import fs from 'fs';

(async () => {
  const browser = await puppeteer.launch({ headless: false });
  const page = await browser.newPage();
  await page.goto('https://www.google.com/?hl=en', {
    waitUntil: 'domcontentloaded',
  });
  const [button] = await page.$x("//button[contains(., 'Accept all')]");
  if (button) {
    await button.click();
  }
  const content = await page.content();
  fs.writeFile('../output/google.html', content, (err) => {
    if (err) {
      console.error(err);
    }
  });

  await browser.close();
})();
